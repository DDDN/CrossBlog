/*
* DDDN.CrossBlog.Blog.Areas.Administration.Controllers.BlogInfoController
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

using DDDN.CrossBlog.Blog.Areas.Administration.Model;
using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Model;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.Areas.Administration.Controllers
{
	[Area("Administration")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class BlogInfoController : Controller
	{
		private readonly IOptions<RoutingConfigSection> _routingConfigSection;
		private readonly CrossBlogContext _ctx;

		public BlogInfoController(
			 IOptions<RoutingConfigSection> routingConfigSection,
			 CrossBlogContext crossBlogContext
			 )
		{
			_routingConfigSection = routingConfigSection ?? throw new System.ArgumentNullException(nameof(routingConfigSection));
			_ctx = crossBlogContext ?? throw new System.ArgumentNullException(nameof(crossBlogContext));
		}

		public async Task<IActionResult> Details()
		{
			_ctx.Database.EnsureCreated();
			var blogInfo = await _ctx.BlogInfo.AsNoTracking().FirstOrDefaultAsync();

			if (blogInfo == default(BlogInfo))
			{
				return RedirectToAction(nameof(Create));
			}
			else
			{
				return View(blogInfo);
			}

		}

		public IActionResult Create()
		{
			_ctx.Database.EnsureCreated();
			var blogInfo = _ctx.BlogInfo.AsNoTracking().FirstOrDefault();

			if (blogInfo != default(BlogInfo))
			{
				return RedirectToAction(nameof(Details));
			}
			else
			{
				return View();
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BlogInfoView blogInfoView)
		{
			if (blogInfoView == null)
			{
				throw new System.ArgumentNullException(nameof(blogInfoView));
			}

			var blogInfo = _ctx.BlogInfo.AsNoTracking().FirstOrDefault();

			if (blogInfo != default(BlogInfo))
			{
				return RedirectToAction(nameof(Details));
			}

			var created = DateTimeOffset.Now;
			var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version.ToString();
			var state = "1";

			blogInfo = new BlogInfo
			{
				Name = blogInfoView.BlogInfoName,
				Copyright = blogInfoView.BlogInfoCopyright,
				Created = created,
				BlogInfoId = Guid.NewGuid(),
				State = state,
				Version = version,
				Writers = new List<Writer>()
			};

			var writer = new Writer
			{
				WriterId = Guid.NewGuid(),
				Created = created,
				Name = blogInfoView.WriterName,
				Mail = blogInfoView.WriterMail,
				Password = Encoding.Unicode.GetBytes(blogInfoView.WriterPassword),
				Salt = Encoding.Unicode.GetBytes(blogInfoView.BlogInfoName + blogInfoView.WriterMail),
				State = state
			};

			blogInfo.Writers.Add(writer);
			_ctx.BlogInfo.Add(blogInfo);
			await _ctx.SaveChangesAsync();

			return RedirectToAction(nameof(Details));
		}
	}
}