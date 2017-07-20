using DDDN.CrossBlog.Blog.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Linq;

namespace DDDN.CrossBlog.Blog.Routing
{
	public class BlogCulturesMiddlewareFilter
	{
		private RequestLocalizationOptions LocalizationOptions { get; set; }

		public void Configure(IApplicationBuilder app, IBlogCultures blogCultures)
		{
			var supportedCultures = blogCultures.Select(p => p.Key).ToList();

			this.LocalizationOptions = new RequestLocalizationOptions()
			{
				DefaultRequestCulture = new RequestCulture(blogCultures.DefaultCulture.Name),
				SupportedCultures = supportedCultures,
				SupportedUICultures = supportedCultures,
				RequestCultureProviders = new[]
				 {
						  new BlogRequestCultureProvider() { Options = LocalizationOptions }
					 }
			};

			app.UseRequestLocalization(LocalizationOptions);
		}
	}
}
