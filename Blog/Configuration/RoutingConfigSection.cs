namespace DDDN.CrossBlog.Blog.Configuration
{
    public class RoutingConfigSection
    {
        public string BlogAreas { get; set; } = "Blog|Dashboard|Configuration";
        public string DefaultRouteTemplate { get; set; } = "{area}/{culture}/{controller}/{action}/{id?}";
        public string DefaultArea { get; set; } = "Blog";
        public string DefaultController { get; set; } = "Home";
        public string DefaultAction { get; set; } = "Index";
        public string CultureRouteDataStringKey { get; set; } = "culture";
    }
}
