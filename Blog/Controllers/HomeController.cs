using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DDDN.CrossBlog.Blog.Configuration;
using Microsoft.Extensions.Options;

namespace DDDN.CrossBlog.Blog.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IOptions<LocalizationSection> localizationSection)
        {
            var ls = localizationSection;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
