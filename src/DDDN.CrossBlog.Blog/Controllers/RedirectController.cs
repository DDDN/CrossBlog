/*
DDDN.CrossBlog.Blog.Controllers.RedirectController
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Localization;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace DDDN.CrossBlog.Blog.Controllers
{
	public class RedirectController : Controller
	{
		private readonly RoutingConfigSection _routingSection;
		private readonly LocalizationConfigSection _localizationSection;
		private readonly IBlogCultures _blogCultures;

		public RedirectController(
			 IOptions<RoutingConfigSection> routingSectionOptions,
			 IOptions<LocalizationConfigSection> localizationSectionOptions,
			 IBlogCultures blogCultures)
		{
			if (routingSectionOptions == null)
			{
				throw new System.ArgumentNullException(nameof(routingSectionOptions));
			}
			_routingSection = routingSectionOptions.Value ?? throw new ArgumentNullException("routingSectionOptions.Value");

			if (localizationSectionOptions == null)
			{
				throw new System.ArgumentNullException(nameof(localizationSectionOptions));
			}
			_localizationSection = localizationSectionOptions.Value ?? throw new ArgumentNullException("localizationSectionOptions.Value");

			_blogCultures = blogCultures ?? throw new ArgumentNullException(nameof(blogCultures));
		}

		public IActionResult Redirect()
		{
			var controller = _routingSection.DefaultController;
			var action = _routingSection.DefaultAction;
			var blogCultureName = _localizationSection.DefaultCulture;
			var returnUrl = HttpContext.Request.Query[_routingSection.ReturnUrl];

			if (!controller.Equals(_routingSection.DefaultController, StringComparison.InvariantCultureIgnoreCase))
			{
				blogCultureName = BlogRequestCultureProvider.GetCultureNameFromRoute(
					HttpContext.Request.Path,
					_routingSection.DefaultRouteTemplate,
					_routingSection.CultureRouteDataStringKey);

				if (string.IsNullOrWhiteSpace(blogCultureName) && !string.IsNullOrEmpty(returnUrl))
				{
					blogCultureName = BlogRequestCultureProvider.GetCultureNameFromRoute(
						returnUrl,
						_routingSection.DefaultRouteTemplate,
						_routingSection.CultureRouteDataStringKey);
					var returnUrlRouteWithCulture = new RouteMatcher().Match(
							_routingSection.DefaultRouteTemplate,
							$"/{blogCultureName}{HttpContext.Request.Path}");
					controller = returnUrlRouteWithCulture["controller"]?.ToString();
					action = returnUrlRouteWithCulture["action"]?.ToString();
				}
			}

			var blogCulture = BlogRequestCultureProvider.GetCulture(blogCultureName, HttpContext, _blogCultures);

			return RedirectToRoute(
				 routeName: RouteNames.Default,
				 routeValues: new
				 {
					 culture = blogCulture.Name,
					 controller = controller,
					 action = action,
					 ReturnUrl = returnUrl
				 });
		}
	}
}
