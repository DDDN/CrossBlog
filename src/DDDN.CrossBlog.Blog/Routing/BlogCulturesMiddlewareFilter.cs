/*
DDDN.CrossBlog.Blog.Routing.BlogCulturesMiddlewareFilter
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

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
