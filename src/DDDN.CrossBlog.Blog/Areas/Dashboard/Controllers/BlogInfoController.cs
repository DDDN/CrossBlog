/*
* DDDN.CrossBlog.Blog.Areas.Dashboard.Controllers.BlogInfoController
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDDN.CrossBlog.Blog.Areas.Dashboard.Data;
using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Model;
using DDDN.CrossBlog.Blog.Model.Data;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DDDN.CrossBlog.Blog.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
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

        public IActionResult Index()
        {
            _ctx.Database.EnsureCreated();
            var blogInfo = _ctx.BlogInfo.AsNoTracking().FirstOrDefault();

            if (blogInfo == default(BlogInfo))
            {
                return RedirectToRoute(
                 routeName: RouteNames.Default,
                 routeValues: new
                 {
                     area = AreaNames.Dashboard,
                     culture = CultureInfo.CurrentCulture,
                     controller = nameof(BlogInfo),
                     action = nameof(Create)
                 });
            }
            else
            {
                return RedirectToRoute(
                 routeName: RouteNames.Default,
                 routeValues: new
                 {
                     area = AreaNames.Dashboard,
                     culture = CultureInfo.CurrentCulture,
                     controller = nameof(BlogInfo),
                     action = nameof(Details)
                 });
            }
        }

        public async Task<IActionResult> Details()
        {
            var blogInfo = await _ctx.BlogInfo.FirstOrDefaultAsync();

            if (blogInfo == default(BlogInfo))
            {
                return NotFound();
            }

            return View(blogInfo);
        }

        public IActionResult Create()
        {
            _ctx.Database.EnsureCreated();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogInfoCreate blogInfoCreate)
        {
            if (blogInfoCreate == null)
            {
                throw new System.ArgumentNullException(nameof(blogInfoCreate));
            }

            var blogInfo = _ctx.BlogInfo.AsNoTracking().FirstOrDefault();

            if (blogInfo != default(BlogInfo))
            {
                return RedirectToRoute(
                 routeName: RouteNames.Default,
                 routeValues: new
                 {
                     area = AreaNames.Dashboard,
                     culture = CultureInfo.CurrentCulture,
                     controller = nameof(BlogInfo),
                     action = nameof(Details)
                 });
            }

            var created = DateTimeOffset.Now;
            var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version.ToString();

            blogInfo = new BlogInfo
            {
                Name = blogInfoCreate.BlogInfoName,
                Copyright = blogInfoCreate.BlogInfoCopyright,
                Created = created,
                BlogInfoId = Guid.NewGuid(),
                State = BlogState.Active,
                Version = version,
                Writers = new List<Writer>()
            };

            var writer = new Writer
            {
                WriterId = Guid.NewGuid(),
                Created = created,
                Name = blogInfoCreate.WriterName,
                Mail = blogInfoCreate.WriterMail,
                Password = Encoding.Unicode.GetBytes(blogInfoCreate.WriterPassword),
                Salt = Encoding.Unicode.GetBytes(blogInfoCreate.BlogInfoName + blogInfoCreate.WriterMail),
                State = WriterState.Active
            };

            blogInfo.Writers.Add(writer);
            _ctx.BlogInfo.Add(blogInfo);
            await _ctx.SaveChangesAsync();

            return View();
        }
    }
}