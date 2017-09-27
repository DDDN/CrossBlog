﻿/*
DDDN.CrossBlog.Blog.Controllers.DashboardController
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.BusinessLayer;
using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Data;
using DDDN.CrossBlog.Blog.Exceptions;
using DDDN.CrossBlog.Blog.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.Controllers
{
	public class DashboardController : CrossBlogController
	{
		private readonly RoutingConfigSection _routingConfig;
		private readonly AuthenticationConfigSection _authenticationConfig;
		private readonly BlogInfo _blogInfo;
		private readonly IStringLocalizer<DashboardController> _loc;
		private readonly IWriterBusinessLayer _writerBl;
		private readonly IBlogBusinessLayer _blogBl;
		private readonly IPostBusinessLayer _postBl;
		private readonly ICategoryBusinessLayer _categoryBl;

		public DashboardController(
			IOptions<RoutingConfigSection> routingConfigSectionOptions,
			IOptions<AuthenticationConfigSection> authenticationConfigSection,
			BlogInfo blogInfo,
			IStringLocalizer<DashboardController> localizer,
			IWriterBusinessLayer writerBusinessLayer,
			IBlogBusinessLayer blogBusinessLayer,
			IPostBusinessLayer postBusinessLayer,
			ICategoryBusinessLayer categoryBusinessLayer)
		{
			if (routingConfigSectionOptions == null)
			{
				throw new ArgumentNullException(nameof(routingConfigSectionOptions));
			}
			_routingConfig = routingConfigSectionOptions.Value ?? throw new ArgumentNullException("routingConfigSectionOptions.Value");

			if (authenticationConfigSection == null)
			{
				throw new ArgumentNullException(nameof(authenticationConfigSection));
			}
			_authenticationConfig = authenticationConfigSection.Value ?? throw new ArgumentNullException("authenticationConfigSection.Value");

			_blogInfo = blogInfo ?? throw new ArgumentNullException(nameof(blogInfo));
			_loc = localizer ?? throw new ArgumentNullException(nameof(localizer));
			_writerBl = writerBusinessLayer ?? throw new ArgumentNullException(nameof(writerBusinessLayer));
			_blogBl = blogBusinessLayer ?? throw new ArgumentNullException(nameof(blogBusinessLayer));
			_postBl = postBusinessLayer ?? throw new ArgumentNullException(nameof(postBusinessLayer));
			_categoryBl = categoryBusinessLayer ?? throw new ArgumentNullException(nameof(categoryBusinessLayer));
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> BlogDetails()
		{
			var blog = await _blogBl.DetailsGet();
			return View(blog);
		}

		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> BlogEdit()
		{
			var blogView = await _blogBl.EditGet();
			return View(blogView);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> BlogEdit([Bind("BlogId, BlogName, BlogCopyright")] BlogViewModel blogViewModel)
		{
			await _blogBl.EditSave(blogViewModel);
			return RedirectToAction(nameof(BlogDetails));
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> PostDetails(Guid id)
		{
			if (id.Equals(Guid.Empty))
			{
				return NotFound();
			}

			var post = await _postBl.GetPostOrDefault(id);

			if (post == default(PostModel))
			{
				return NotFound();
			}

			return View(post);
		}
		/// <summary>
		/// Shows a table with posts infromations.
		/// </summary>
		/// <returns>List of post models.</returns>
		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> Posts()
		{
			var posts = await _postBl.GetNewest(0, int.MaxValue);
			return View(posts);
		}
		/// <summary>
		/// Renders post information for post deletion.
		/// </summary>
		/// <param name="id">Post id.</param>
		/// <returns>Post model.</returns>
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> PostDelete(Guid id)
		{
			if (id.Equals(Guid.Empty))
			{
				return NotFound();
			}

			var post = await _postBl.GetPostOrDefault(id);

			if (post == default(PostModel))
			{
				return NotFound();
			}

			return View(post);
		}

		/// <summary>
		/// Delete a post and redirect to the index action.
		/// </summary>
		/// <param name="id">Post id.</param>
		/// <returns>Redirection to Index action.</returns>
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> PostDeleteConfirmed(Guid id)
		{
			if (id.Equals(Guid.Empty))
			{
				return NotFound();
			}

			await _postBl.Delete(id);
			return RedirectToAction(nameof(Posts));
		}

		/// <summary>
		/// Shows the ODT upload dialog.
		/// </summary>
		/// <returns></returns>
		[Authorize(Roles = "Writer")]
		public IActionResult PostUpload()
		{
			return View();
		}

		/// <summary>
		/// Uploads one or more ODT files and convert is into a post.
		/// </summary>
		/// <param name="files">List of ODT files tat will be converted into posts.</param>
		/// <returns></returns>
		[Authorize(Roles = "Writer")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> PostUpload(IList<IFormFile> files)
		{
			if (files == null)
			{
				throw new ArgumentNullException(nameof(files));
			}

			if (files.Any())
			{
				await _postBl.Upload(files);
				return RedirectToAction(nameof(Posts));
			}

			return NotFound();
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> PostEdit(Guid id)
		{
			if (id.Equals(Guid.Empty))
			{
				return NotFound();
			}

			var post = await _postBl.GetWithCategories(id);
			var categories = await _categoryBl.Get();

			var postView = new PostViewModel(post, categories, _loc)
			{
				PostId = post.PostId,
				State = post.State,
				FirstHeader = post.FirstHeader,
				FirstParagraph = post.FirstParagraph,
				AlternativeTitle = post.AlternativeTitle,
				AlternativeTeaser = post.AlternativeTeaser,
			};

			return View(postView);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> PostEdit(Guid id, [Bind("PostId,State,AlternativeTitle,AlternativeTeaser")] PostViewModel postViewModel)
		{
			if (!postViewModel.PostId.Equals(id))
			{
				return NotFound();
			}

			var categoryIds = Request.Form[nameof(PostViewModel.Categories)].ToList<string>();
			await _postBl.Edit(postViewModel, categoryIds);
			return View(postViewModel);
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> Categories()
		{
			var categories = await _categoryBl.Get();
			return View(categories);
		}

		[HttpPost]
		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> CategoriesCreate(string name)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				await _categoryBl.Create(name);
			}

			return RedirectToAction(nameof(Categories));
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> CategoriesDelete(Guid CategoryId)
		{
			if (CategoryId.Equals(Guid.Empty))
			{
				return NotFound();
			}

			await _categoryBl.Delete(CategoryId);
			return RedirectToAction(nameof(Categories));
		}

		[AllowAnonymous]
		public IActionResult Login(string returnUrl = null)
		{
			var logonView = new LoginViewModel
			{
				ReturnUrl = returnUrl
			};

			return View(logonView);
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login([Bind("Mail, Password")]LoginViewModel loginViewModel, string returnUrl = null)
		{
			var authentication = await _writerBl.TryToAuthenticateAndGetPrincipal(loginViewModel.Mail, loginViewModel.Password);

			if (authentication.authenticationResult == AuthenticationResult.Authenticated)
			{
				var props = new AuthenticationProperties
				{
					AllowRefresh = true,
					IsPersistent = true
				};

				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authentication.principal, props);
				return base.RedirectToLocal(returnUrl, _routingConfig.DefaultController, _routingConfig.DefaultAction);
			}
			else if (authentication.authenticationResult == AuthenticationResult.WrongPassword)
			{
				return RedirectToLocal(returnUrl, _routingConfig.DefaultController, _routingConfig.DefaultAction);
			}
			else if (authentication.authenticationResult == AuthenticationResult.UserNotFound)
			{
				return RedirectToLocal(returnUrl, _routingConfig.DefaultController, _routingConfig.DefaultAction);
			}
			else
			{
				return RedirectToLocal(returnUrl, _routingConfig.DefaultController, _routingConfig.DefaultAction);
			}
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction(nameof(BlogController.Newest), "Blog");
		}

		public async Task<IActionResult> Writers()
		{
			var writers = await _writerBl.GetWithRoles();
			return View(writers);
		}

		public async Task<IActionResult> WriterDetails(Guid id)
		{
			WriterModel writer = null;

			try
			{
				writer = await _writerBl.GetWithRoles(id);
			}
			catch (WriterNotFoundException)
			{
				return NotFound();
			}

			var writerView = new WriterViewModel(writer.State, _loc)
			{
				WriterId = writer.WriterId,
				State = writer.State,
				Mail = writer.Mail,
				Name = writer.Name,
				AboutMe = writer.AboutMe,
				Created = writer.Created,
				Administrator = writer.Roles.Any(p => p.Role.Equals(RoleModel.Roles.Administrator))
			};

			return View(writerView);
		}

		[Authorize(Roles = "Administrator")]
		public IActionResult WriterCreate()
		{
			var writerView = new WriterViewModel();
			return View(writerView);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> WriterCreate([Bind("Name, Mail, AboutMe, Administrator, Password, PasswordCompare")] WriterViewModel writerView)
		{
			if (_writerBl.MailExist(writerView.Mail))
			{
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}

			await _writerBl.Create(writerView);
			return RedirectToAction(nameof(Writers));
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> WriterEdit(Guid id)
		{
			WriterModel writer = null;

			try
			{
				writer = await _writerBl.GetWithRoles(id);
			}
			catch (WriterNotFoundException)
			{
				NotFound();
			}

			var writerView = new WriterViewModel(writer.State, _loc)
			{
				WriterId = writer.WriterId,
				State = writer.State,
				Mail = writer.Mail,
				Name = writer.Name,
				AboutMe = writer.AboutMe,
				Administrator = writer.Roles.Any(p => p.Role.Equals(RoleModel.Roles.Administrator))
			};

			return View(writerView);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> WriterEdit(Guid id, [Bind("WriterId, State, Name, Mail, AboutMe, Administrator")]WriterViewModel writerView)
		{
			if (id != writerView.WriterId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				await _writerBl.Update(writerView);
			}

			return RedirectToAction(nameof(Writers));
		}

		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> WriterDelete(Guid id)
		{
			if (_writerBl.IsOwner(id))
			{
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}

			WriterModel writer = null;

			try
			{
				writer = await _writerBl.GetWithRoles(id);
			}
			catch (WriterNotFoundException)
			{
				return NotFound();
			}

			var writerView = new WriterViewModel(writer.State, _loc)
			{
				WriterId = writer.WriterId,
				State = writer.State,
				Mail = writer.Mail,
				Name = writer.Name,
				AboutMe = writer.AboutMe,
				Created = writer.Created,
				Administrator = writer.Roles.Any(p => p.Role.Equals(RoleModel.Roles.Administrator))
			};

			return View(writerView);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost, ActionName("WriterDelete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> WriterDeleteConfirmed(Guid id)
		{
			if (_writerBl.IsOwner(id))
			{
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}

			try
			{
				await _writerBl.Delete(id);
			}
			catch (WriterNotFoundException)
			{
				NotFound();
			}

			return RedirectToAction(nameof(Writers));
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> WriterPassword(Guid id)
		{
			WriterModel writer = null;

			try
			{
				writer = await _writerBl.GetWithRoles(id);
			}
			catch (WriterNotFoundException)
			{

				return NotFound();
			}

			var pwView = new PasswordViewModel
			{
				WriterId = writer.WriterId
			};

			return View(pwView);
		}

		[HttpPost]
		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> WriterPasswordConfirmed([Bind("WriterId, Old, New, Compare")] PasswordViewModel passwordViewModel)
		{
			var result = await _writerBl.PasswordChange(passwordViewModel);

			if (result == AuthenticationResult.PasswordChanged)
			{
				return RedirectToAction(nameof(Writers));
			}
			else if (result == AuthenticationResult.UserNotFound)
			{
				return NotFound();
			}
			else
			{
				return RedirectToAction(nameof(WriterPassword));
			}
		}
	}
}
