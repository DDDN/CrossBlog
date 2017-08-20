/*
* DDDN.CrossBlog.Blog.Areas.Dashboard.Controllers.BlogController
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
using DDDN.CrossBlog.Blog.Model;
using DDDN.CrossBlog.Blog.Model.Data;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq;

namespace DDDN.CrossBlog.Blog.Areas.Dashboard.Controllers
{
	[Area("Dashboard")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class BlogController : Controller
	{
		private readonly IStringLocalizer _homeLocalizer;
		private readonly IOptions<RoutingConfigSection> _routingConfigSection;
		private readonly CrossBlogContext _db;

		public BlogController(
			IStringLocalizer<HomeController> homeLocalizer,
			IOptions<RoutingConfigSection> routingConfigSection,
			CrossBlogContext crossBlogContext
			)
		{
			_homeLocalizer = homeLocalizer ?? throw new System.ArgumentNullException(nameof(homeLocalizer));
			_routingConfigSection = routingConfigSection ?? throw new System.ArgumentNullException(nameof(routingConfigSection));
			_db = crossBlogContext ?? throw new System.ArgumentNullException(nameof(crossBlogContext));
		}

		public IActionResult Index()
		{
			//CrossBlogDbInitializer.Initialize(_db);
			var blog = _db.BlogInfo.FirstOrDefault();

			if (blog == default(BlogInfo))
			{
				return RedirectToRoute(
				 routeName: RouteNames.Default,
				 routeValues: new
				 {
					 area = AreaNames.Dashboard,
					 culture = CultureInfo.CurrentCulture,
					 controller = "Blog",
					 action = "Create"
				 });
			}

			return View();
		}

		public IActionResult Create()
		{
			return View();
		}
	}
}