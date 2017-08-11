using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using Xunit;

namespace DDDN.CrossBlog.Blog.Tests.Routing
{
	public class RouteMatcherTests
    {
        [Fact]
        public void Match_MatchDefaultRouteTemplate()
        {
            // Arrange
            var router = new RouteMatcher();
            var defaultTemplate = "{area}/{culture}/{controller}/{action}/{id?}";
            var path = new Uri("/Blog/en-US/Home/Index", UriKind.Relative).ToString();
            var routeValues = new RouteValueDictionary()
            {
                ["area"] = "Blog",
                ["culture"] = "en-US",
                ["controller"] = "Home",
                ["action"] = "Index"
            };

            // Act
            var result = router.Match(defaultTemplate, path);

            // Assert
            Assert.True(result.All(e => routeValues.Contains(e)));
        }
    }
}
