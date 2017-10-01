/*
DDDN.CrossBlog.Blog.Controllers.BlogController
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.BusinessLayer;
using DDDN.CrossBlog.Blog.Exceptions;
using DDDN.CrossBlog.Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.Controllers
{
	public class BlogController : CrossBlogController
	{
		private readonly IPostBusinessLayer _postBl;
		private readonly IBlogBusinessLayer _blogBl;

		public BlogController(
			IPostBusinessLayer postBusinessLayer,
			IBlogBusinessLayer blogBusinessLayer)
		{
			_postBl = postBusinessLayer ?? throw new System.ArgumentNullException(nameof(postBusinessLayer));
			_blogBl = blogBusinessLayer ?? throw new System.ArgumentNullException(nameof(blogBusinessLayer));
		}

		public async Task<IActionResult> Newest(Guid id)
		{
			if (id.Equals(Guid.Empty))
			{
				var posts = await _postBl.GetNewest(0, 1, PostModel.States.Published);
				return View(posts);
			}
			else
			{
				var posts = await _postBl.GetNewestByCategory(0, 10, id, PostModel.States.Published);
				return View(posts);
			}
		}

		public async Task<IActionResult> Show(Guid id)
		{
			var post = await _postBl.GetPostWithCommentsOrDefault(id);

			if (post != default(PostModel))
			{
				return View(post);
			}
			else
			{
				return NotFound();
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CommentAdd(Guid PostId, string personName, string commentTitle, string commentText)
		{
			await _postBl.CommentSave(PostId, personName, commentTitle, commentText);
			return RedirectToAction(nameof(Show));
		}

		public async Task<IActionResult> PostContent(Guid id, string filename)
		{
			try
			{
				var content = await _postBl.GetContent(id);
				var contentType = new FileExtensionContentTypeProvider().Mappings[Path.GetExtension(content.name)];
				var result = new FileContentResult(content.binary, contentType ?? "application/octet-stream");
				return result;
			}
			catch (PostContentNotFoundException)
			{
				return NotFound();
			}
		}

		public async Task<IActionResult> Archive()
		{
			return View();
		}

		public async Task<IActionResult> Categories()
		{
			return View();
		}

		public async Task<IActionResult> About()
		{
			return View();
		}
	}
}