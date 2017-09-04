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
using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Routing;
using DDDN.Office.Odf.Odt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
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
		private readonly IOptions<RoutingConfigSection> _routingConfigSection;
		private readonly CrossBlogContext _context;
		private readonly IStringLocalizer<PostsController> _localizer;


		public PostsController(
			IOptions<RoutingConfigSection> routingConfigSection,
			CrossBlogContext context,
			IStringLocalizer<PostsController> localizer)
		{
			_routingConfigSection = routingConfigSection ?? throw new ArgumentNullException(nameof(routingConfigSection));
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
								var convertedData = new ODTConvert(odtFile, _routingConfigSection.Value.BlogPostHtmlUrlPrefix).Convert();

								var now = DateTimeOffset.Now;

								var post = new Post
								{
									PostId = Guid.NewGuid(),
									State = Post.States.Uploaded,
									Created = now,
									Binary = fileContentBytes,
									Hash = sha1Hash,
									FirstHeaderText = convertedData.FirstHeaderText,
									FirstParagraphHtml = convertedData.FirstParagraphHtml,
									Html = convertedData.Html,
									Css = convertedData.Css,
									Writer = _context.Writers.First(),
									Contents = new List<Content>(),
									LastRenderd = now
								};

								foreach (var ec in convertedData.EmbedContent)
								{
									var content = new Content
									{
										Binary = ec.Content,
										Created = now,
										Name = ec.Name,
										State = CrossBlog.Blog.Models.Content.States.Visible,
										ContentId = ec.Id,
										Post = post
									};

									post.Contents.Add(content);
								}

								posts.Add(post);
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
				.Include(p => p.PostCategories)
				.SingleOrDefaultAsync(m => m.PostId == id);

			var categories = await _context.Categories.ToListAsync();

			if (post == default(Post))
			{
				return NotFound();
			}

			var postView = new PostView(post, categories, _localizer)
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
		public async Task<IActionResult> Edit(Guid id, [Bind("PostId,State,AlternativeTitleText,AlternativeTeaserText")] PostView postView)
		{
			if (id != postView.PostId)
			{
				return NotFound();
			}

			var post = await _context.Posts
				.Include(p => p.PostCategories)
				.SingleOrDefaultAsync(m => m.PostId == id);

			post.State = postView.State;
			post.AlternativeTitleText = postView.AlternativeTitleText;
			post.AlternativeTeaserText = postView.AlternativeTeaserText;

			if (post.State == Post.States.Published)
			{
				if (post.FirstPublished == null)
				{
					post.FirstPublished = postView.Published;
				}

				post.LastPublished = postView.Published;
			}

			var catIdStrings = Request.Form[nameof(PostView.Categories)].ToList<string>();

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
