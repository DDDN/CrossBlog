/*
* DDDN.CrossBlog.Blog.Areas.Blog.Controllers.HomeController
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace DDDN.CrossBlog.Blog.Areas.Blog.Controllers
{
	[Area("Blog")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class HomeController : Controller
	{
		private readonly IStringLocalizer _homeLocalizer;
		private readonly IOptions<BlogConfigSection> _blogConfigSection;
		private readonly CrossBlogContext _cxt;

		public HomeController(
			 IStringLocalizer<HomeController> homeLocalizer,
			 IOptions<BlogConfigSection> blogConfigSection,
			 CrossBlogContext crossBlogContext
			 )
		{
			_homeLocalizer = homeLocalizer ?? throw new System.ArgumentNullException(nameof(homeLocalizer));
			_blogConfigSection = blogConfigSection ?? throw new System.ArgumentNullException(nameof(blogConfigSection));
			_cxt = crossBlogContext ?? throw new System.ArgumentNullException(nameof(crossBlogContext));
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

		public IActionResult Archive()
		{
			ViewBag.Me = HttpContext.Request.Path;

			return View();
		}

		public IActionResult About()
		{
			ViewBag.Me = HttpContext.Request.Path;

			return View();
		}
	}
}