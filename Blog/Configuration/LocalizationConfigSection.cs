namespace DDDN.CrossBlog.Blog.Configuration
{
    public class LocalizationConfigSection
    {
        public string DefaultCulture { get; set; }
        public bool OverrideDefaultCultureByAcceptLanguageHeader { get; set; }
        public string SupportedCultures { get; set; }
        public string DefaultForNaturalCulturesSuffix { get; set; } = "*";
    }
}
