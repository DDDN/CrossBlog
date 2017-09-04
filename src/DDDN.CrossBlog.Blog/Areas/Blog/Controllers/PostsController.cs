/*
* DDDN.CrossBlog.Blog.Areas.Blog.Controllers.PostsController
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

using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace DDDN.CrossBlog.Blog.Areas.Blog.Controllers
{
	[Area("Blog")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class PostsController : Controller
	{
		private readonly IStringLocalizer _homeLocalizer;
		private readonly IOptions<BlogConfigSection> _blogConfigSection;
		private readonly CrossBlogContext _cxt;

		public PostsController(
			 IStringLocalizer<PostsController> homeLocalizer,
			 IOptions<BlogConfigSection> blogConfigSection,
			 CrossBlogContext crossBlogContext
			 )
		{
			_homeLocalizer = homeLocalizer ?? throw new ArgumentNullException(nameof(homeLocalizer));
			_blogConfigSection = blogConfigSection ?? throw new ArgumentNullException(nameof(blogConfigSection));
			_cxt = crossBlogContext ?? throw new ArgumentNullException(nameof(crossBlogContext));
		}

		public IActionResult Index()
		{
			var tenNewestPosts = _cxt.Posts
				 .AsNoTracking()
				 .Include(p => p.Writer)
				 .OrderByDescending(p => p.Created)
				 .Take(10);

			return View(tenNewestPosts);
		}

		public IActionResult Show(Guid id)
		{
			var post = _cxt.Posts.AsNoTracking().Where(p => p.PostId == id).FirstOrDefault();

			if (post != default(Post))
			{
				return View(post);
			}
			else
			{
				return NotFound();
			}
		}

		public IActionResult PostContent(Guid id, string filename)
		{
			var content = _cxt.Contents.Where(p => p.ContentId.Equals(id)).FirstOrDefault();

			if (content == default(Content))
			{
				return NotFound();
			}
			else
			{
				var contentType = new FileExtensionContentTypeProvider().Mappings[Path.GetExtension(content.Name)];
				var result = new FileContentResult(content.Binary, contentType ?? "application/octet-stream");
				return result;
			}
		}
	}
}