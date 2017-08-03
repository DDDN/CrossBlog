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

using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace DDDN.CrossBlog.Blog.Areas.Blog.Controllers
{
	[Area("Blog")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class HomeController : Controller
	{
		public IStringLocalizer _homeLocalizer;

		public HomeController(IStringLocalizer<HomeController> homeLocalizer)
		{
			_homeLocalizer = homeLocalizer ?? throw new System.ArgumentNullException(nameof(homeLocalizer));
		}

		public IActionResult Index()
		{
			ViewBag.Me = HttpContext.Request.Path;
			ViewBag.CurrentCultureName = CultureInfo.CurrentCulture.Name;
			ViewBag.Test1 = _homeLocalizer["Test1"];

			return View();
		}

		public IActionResult Newest()
		{
			ViewBag.Me = HttpContext.Request.Path;

			return View();
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