using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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