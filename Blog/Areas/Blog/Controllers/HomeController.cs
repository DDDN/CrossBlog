using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Globalization;

namespace DDDN.CrossBlog.Blog.Areas.Blog.Controllers
{
    [Area("Blog")]
    [MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
    public class HomeController : Controller
    {
        public IHttpContextAccessor _httpContextAccessor;
        private IActionDescriptorCollectionProvider _descriptorCollectionProvider;

        public HomeController(
            IHttpContextAccessor httpContextAccessor,
            IActionDescriptorCollectionProvider descriptorCollectionProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _descriptorCollectionProvider = descriptorCollectionProvider;
        }

        public IActionResult Index()
        {
            ViewBag.CurrentCultureName = CultureInfo.CurrentCulture.Name;

            return View();
        }
    }
}