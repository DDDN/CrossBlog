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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace DDDN.CrossBlog.Blog.Controllers
{
	public class RedirectController : Controller
	{
		private readonly RoutingConfigSection _routingConfig;
		private readonly LocalizationConfigSection _localizationConfig;
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
			_routingConfig = routingSectionOptions.Value ?? throw new ArgumentNullException("routingSectionOptions.Value");

			if (localizationSectionOptions == null)
			{
				throw new System.ArgumentNullException(nameof(localizationSectionOptions));
			}
			_localizationConfig = localizationSectionOptions.Value ?? throw new ArgumentNullException("localizationSectionOptions.Value");

			_blogCultures = blogCultures ?? throw new ArgumentNullException(nameof(blogCultures));
		}

		public IActionResult Redirect()
		{
			var culture = _localizationConfig.DefaultCulture;
			var controller = _routingConfig.DefaultController;
			var action = _routingConfig.DefaultAction;
			var returnUrl = HttpContext.Request.Query[_routingConfig.ReturnUrl].FirstOrDefault();

			if (returnUrl != default(string))
			{
				var route = new RouteMatcher().Match(_routingConfig.DefaultRouteTemplate, HttpContext.Request.Path);

				if (!route.Any())
				{
					var cultureNameFromReturnUrl = BlogRequestCultureProvider.GetCultureNameFromRoute(
						returnUrl,
						_routingConfig.DefaultRouteTemplate,
						_routingConfig.CultureRouteDataStringKey);

					route = new RouteMatcher().Match(
						_routingConfig.DefaultRouteTemplate,
						$"/{cultureNameFromReturnUrl}{HttpContext.Request.Path}");

					if (route.Any())
					{
						culture = route[_routingConfig.CultureRouteDataStringKey]?.ToString();
						controller = route["controller"]?.ToString();
						action = route["action"]?.ToString();
					}
				}
			}
			else
			{
				var route = new RouteMatcher().Match(_routingConfig.DefaultRouteTemplate, HttpContext.Request.Path);

				if (route.Any())
				{
					culture = route[_routingConfig.CultureRouteDataStringKey]?.ToString();
					controller = route["controller"]?.ToString();
					action = route["action"]?.ToString();
				}
			}

			var blogCulture = BlogRequestCultureProvider.GetCulture(culture, HttpContext, _blogCultures);

			var redirect = RedirectToRoute(
				 routeName: RouteNames.Default,
				 routeValues: new
				 {
					 culture = blogCulture.Name,
					 controller = controller,
					 action = action,
					 ReturnUrl = returnUrl
				 });

			return redirect;
		}
	}
}
