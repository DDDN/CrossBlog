/*
* DDDN.CrossBlog.Blog.Areas.Blog.Controllers.RedirectController
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
using DDDN.CrossBlog.Blog.Localization;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DDDN.CrossBlog.Blog.Areas.Redirect.Controllers
{
	[Area("Redirect")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class RedirectController : Controller
	{
		private readonly RoutingConfigSection _routingSection;
		private readonly IBlogCultures _blogCultures;

		public RedirectController(
			 IOptions<RoutingConfigSection> routingSection,
			 IBlogCultures blogCultures)
		{
			_routingSection = routingSection.Value;
			_blogCultures = blogCultures;
		}

		public IActionResult Redirect()
		{
			var blogCultureName = BlogRequestCultureProvider.GetCultureNameFromDefaultRoute(
				 HttpContext.Request.Path,
				 _routingSection.DefaultRouteTemplate,
				 _routingSection.CultureRouteDataStringKey);
			var blogCulture = BlogRequestCultureProvider.GetCulture(blogCultureName, HttpContext, _blogCultures);

			return RedirectToRoute(
				 routeName: RouteNames.Default,
				 routeValues: new
				 {
					 area = _routingSection.DefaultArea,
					 culture = blogCulture.Name,
					 controller = _routingSection.DefaultController,
					 action = _routingSection.DefaultAction
				 });
		}
	}
}
