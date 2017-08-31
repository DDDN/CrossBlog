/*
* DDDN.CrossBlog.Blog.Areas.Dashboard.Controllers.PostsController
* 
* Copyright(C) 2017 Lukasz Jaskiewicz
* Author: Lukasz Jaskiewicz (lukasz@jaskiewicz.de, devdone@outlook.com)
*
* This program is free software; you can redistribute it and/or modify it under the terms of the
* GNU General Public License as published by the Free Software Foundation; version 2 of the License.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
* warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program; if not, write
* to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Areas.Dashboard.Models;
using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Routing;
using DDDN.Office.Odf.Odt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.Areas.Dashboard.Controllers
{
	[Area("Dashboard")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class PostsController : Controller
	{
		private readonly CrossBlogContext _context;
		IStringLocalizer<PostsController> _localizer;


		public PostsController(
			CrossBlogContext context,
			IStringLocalizer<PostsController> localizer)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.Posts.ToListAsync());
		}

		public async Task<IActionResult> Details(Guid? id)
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

		public IActionResult Upload()
		{
			var uploadedPosts = _context.Posts
				 .Where(p => p.State == Post.States.Published);

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Upload(IList<IFormFile> files)
		{
			List<Post> posts = new List<Post>();

			if (ModelState.IsValid)
			{
				using (var sha1 = new SHA1Managed())
				{
					foreach (var file in files)
					{
						using (var fileContent = new MemoryStream())
						{
							await file.CopyToAsync(fileContent);
							var fileContentBytes = fileContent.ToArray();
							var sha1Hash = sha1.ComputeHash(fileContentBytes);

							using (IODTFile odtFile = new ODTFile(fileContent))
							{
								var convertedData = new ODTConvert(odtFile).Convert();

								var post = new Post
								{
									PostId = Guid.NewGuid(),
									State = Post.States.Uploaded,
									Created = DateTimeOffset.Now,
									Binary = fileContentBytes,
									Hash = sha1Hash,
									FirstHeaderText = convertedData.FirstHeaderText,
									FirstParagraphHtml = convertedData.FirstParagraphHtml,
									Html = convertedData.Html,
									Css = convertedData.Css,
									Writer = _context.Writers.First()
								};

								_context.AddRange(post);
							}
						}
					}
				}

				_context.AddRange(posts);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			else
			{
				return View();
			}
		}

		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var post = await _context.Posts
				.AsNoTracking()
				.SingleOrDefaultAsync(m => m.PostId == id);

			if (post == default(Post))
			{
				return NotFound();
			}

			var postView = new PostView(_localizer)
			{
				PostId = post.PostId,
				Created = post.Created,
				State = post.State,
				FirstHeaderText = post.FirstHeaderText,
				FirstParagraphHtml = post.FirstParagraphHtml,
				AlternativeTitle = post.AlternativeTitle,
				AlternativeTeaser = post.AlternativeTeaser
			};

			return View(postView);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("PostId,State,Created,AlternativeTitle,AlternativeTeaser")] PostView postView)
		{
			if (id != postView.PostId)
			{
				return NotFound();
			}

			var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId == id);
			post.Created = postView.Created;
			post.State = postView.State;
			post.AlternativeTitle = postView.AlternativeTitle;
			post.AlternativeTeaser = postView.AlternativeTeaser;

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

				return RedirectToAction(nameof(Index));
			}

			return View(postView);
		}

		public async Task<IActionResult> Delete(Guid? id)
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

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId == id);
			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool PostExists(Guid id)
		{
			return _context.Posts.Any(e => e.PostId == id);
		}
	}
}
