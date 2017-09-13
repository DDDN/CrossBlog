/*
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
using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Security;
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
		private readonly CrossBlogContext _context;
		private readonly IStringLocalizer<DashboardController> _loc;
		private readonly IWriterBusinessLayer _writerBl;
		private readonly IBlogBusinessLayer _blogBl;
		private readonly IPostBusinessLayer _postBl;

		public DashboardController(
			IOptions<RoutingConfigSection> routingConfigSectionOptions,
			IOptions<AuthenticationConfigSection> authenticationConfigSection,
			CrossBlogContext context,
			IStringLocalizer<DashboardController> localizer,
			IWriterBusinessLayer writerBusinessLayer,
			IBlogBusinessLayer blogBusinessLayer,
			IPostBusinessLayer postBusinessLayer)
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

			_context = context ?? throw new ArgumentNullException(nameof(context));
			_loc = localizer ?? throw new ArgumentNullException(nameof(localizer));
			_writerBl = writerBusinessLayer ?? throw new ArgumentNullException(nameof(writerBusinessLayer));
			_blogBl = blogBusinessLayer ?? throw new ArgumentNullException(nameof(blogBusinessLayer));
			_postBl = postBusinessLayer ?? throw new ArgumentNullException(nameof(postBusinessLayer));
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
		public async Task<IActionResult> PostDetails(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var post = await _context.Posts
				  .SingleOrDefaultAsync(m => m.PostId == id);
			if (post == null)
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
			var posts = await _context.Posts.AsNoTracking().ToListAsync();
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

			var post = await _context.Posts
				  .SingleOrDefaultAsync(m => m.PostId == id);

			if (post == null)
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
			var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId == id);
			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Posts));
		}

		/// <summary>
		/// Shows the ODT upload dialog.
		/// </summary>
		/// <returns></returns>
		[Authorize(Roles = "Writer")]
		public IActionResult PostUpload()
		{
			var uploadedPosts = _context.Posts
				 .Where(p => p.State == PostModel.States.Published);

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
			if (ModelState.IsValid)
			{
				await _postBl.Upload(files);
				return RedirectToAction(nameof(Posts));
			}
			else
			{
				return View();
			}
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> PostEdit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var post = await _context.Posts
				.AsNoTracking()
				.Include(p => p.PostCategories)
				.SingleOrDefaultAsync(m => m.PostId == id);

			var categories = await _context.Categories.ToListAsync();

			if (post == default(PostModel))
			{
				return NotFound();
			}

			var postView = new PostViewModel(post, categories, _loc)
			{
				PostId = post.PostId,
				State = post.State,
				FirstHeaderText = post.FirstHeaderText,
				FirstParagraphHtml = post.FirstParagraphHtml,
				AlternativeTitleText = post.AlternativeTitleText,
				AlternativeTeaserText = post.AlternativeTeaserText,
			};

			return View(postView);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> PostEdit(Guid id, [Bind("PostId,State,AlternativeTitleText,AlternativeTeaserText")] PostViewModel postViewModel)
		{
			if (id != postViewModel.PostId)
			{
				return NotFound();
			}

			var post = await _context.Posts
				.Include(p => p.PostCategories)
				.SingleOrDefaultAsync(m => m.PostId == id);

			post.State = postViewModel.State;
			post.AlternativeTitleText = postViewModel.AlternativeTitleText;
			post.AlternativeTeaserText = postViewModel.AlternativeTeaserText;

			if (post.State == PostModel.States.Published)
			{
				if (post.FirstPublished == null)
				{
					post.FirstPublished = postViewModel.Published;
				}

				post.LastPublished = postViewModel.Published;
			}

			var catIdStrings = Request.Form[nameof(PostViewModel.Categories)].ToList<string>();

			for (int i = post.PostCategories.Count() - 1; i >= 0; i--)
			{
				var cat = post.PostCategories.ElementAt(i);

				if (catIdStrings.Contains(cat.CategoryId.ToString()))
				{
					catIdStrings.Remove(cat.CategoryId.ToString());
				}
				else
				{
					post.PostCategories.Remove(cat);
				}
			}

			var newCats = new List<PostCategoryMap>();

			foreach (var catIdStr in catIdStrings)
			{
				var catId = Guid.Parse(catIdStr);

				if (!post.PostCategories.Where(p => p.CategoryId.Equals(catId)).Any())
				{
					var pm = new PostCategoryMap
					{
						CategoryId = catId,
						PostId = post.PostId
					};

					newCats.Add(pm);
				}
			}

			post.PostCategories.AddRange(newCats);

			if (ModelState.IsValid)
			{
				try
				{
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!PostExists(post.PostId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}

				return RedirectToAction(nameof(Posts));
			}

			return View(postViewModel);
		}

		private bool PostExists(Guid id)
		{
			return _context.Posts.Any(e => e.PostId == id);
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> Categories()
		{
			var categoryNames = await _context.Categories.AsNoTracking().ToListAsync();
			return View(categoryNames);
		}

		[HttpPost]
		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> CategoriesCreate(string name)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				var category = new CategoryModel
				{
					CategoryId = Guid.NewGuid(),
					Name = name
				};

				_context.Categories.Add(category);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Categories));
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> CategoriesDelete(Guid CategoryId)
		{
			var category = await _context.Categories.Where(p => p.CategoryId.Equals(CategoryId)).FirstOrDefaultAsync();

			if (category != default(CategoryModel))
			{
				_context.Remove(category);
				await _context.SaveChangesAsync();
			}

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
			var writers = await _writerBl.GetWritersWithRoles();
			return View(writers);
		}

		public async Task<IActionResult> WriterDetails(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var writer = await _context.Writers
				 .SingleOrDefaultAsync(m => m.WriterId == id);
			if (writer == null)
			{
				return NotFound();
			}

			return View(writer);
		}

		[Authorize(Roles = "Administrator")]
		public IActionResult WriterCreate()
		{
			return View(new WriterViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> WriterCreate([Bind("Name, Mail, AboutMe, Administrator, Password, PasswordCompare")] WriterViewModel writerView)
		{
			await _writerBl.WriterCreate(writerView);
			return RedirectToAction(nameof(Writers));
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> WriterEdit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var writer = await _context.Writers
				.AsNoTracking()
				.Where(p => p.WriterId.Equals(id))
				.Include(p => p.Roles)
				.SingleOrDefaultAsync();

			if (writer == default(WriterModel))
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
				await _writerBl.WriterEdit(writerView);
			}

			return RedirectToAction(nameof(Writers));
		}

		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> WriterDelete(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var writer = await _context.Writers
				 .SingleOrDefaultAsync(m => m.WriterId == id);
			if (writer == null)
			{
				return NotFound();
			}

			return View(writer);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> WriterDeleteConfirmed(Guid id)
		{
			var writer = await _context.Writers.SingleOrDefaultAsync(m => m.WriterId == id);
			_context.Writers.Remove(writer);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Writers));
		}

		private bool WriterExists(Guid id)
		{
			return _context.Writers.Any(e => e.WriterId == id);
		}

		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> WriterPassword(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var writer = await _context.Writers
				 .SingleOrDefaultAsync(m => m.WriterId == id);
			if (writer == null)
			{
				return NotFound();
			}

			return View(writer);
		}
	}
}
