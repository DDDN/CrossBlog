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
