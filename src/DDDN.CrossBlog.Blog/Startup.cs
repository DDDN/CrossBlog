/*
* DDDN.CrossBlog.Blog.Startup
* 
* Copyright(C) 2017 Lukasz Jaskiewicz
* Author: Lukasz Jaskiewicz (lukasz@jaskiewicz.de, devdone@outlook.com)
*
* This program is free software; you can redistribute it and/or modify it under the terms of the
* GNU General Public License as published by the Free Software Foundation; version 2 of the License.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
* warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program; if not, write
* to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Localization;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DDDN.CrossBlog.Blog
{
	public class Startup
	{
		public IConfiguration Config { get; }

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				  .SetBasePath(env.ContentRootPath)
				  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
			Config = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			var localizationConfigSection = Config.GetSection(ConfigSectionNames.Localization);
			var stringResourceFolder = localizationConfigSection.Get<LocalizationConfigSection>().StringResourceFolder;

			services.AddOptions();
			services.Configure<LocalizationConfigSection>(localizationConfigSection);
			services.Configure<LocalizationConfigSection>(Config.GetSection(ConfigSectionNames.Routing));
			services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<LocalizationConfigSection>>().Value);
			services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<RoutingConfigSection>>().Value);
			services.TryAddSingleton<IStringLocalizerFactory, BlogStringLocalizerFactory>();
			services.TryAddSingleton<IStringLocalizer, BlogStringLocalizer>();
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.TryAddSingleton<IBlogCultures, BlogCultures>();
			services.AddLocalization(options => options.ResourcesPath = stringResourceFolder);
			services.AddRouting(options => options.LowercaseUrls = false);
			services.AddMvc()
				 .AddViewLocalization();
		}

		public void Configure(
			  IApplicationBuilder app,
			  IHostingEnvironment env,
			  IBlogCultures blogCultures,
			  IOptions<RoutingConfigSection> routingSection)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute
						 (
								name: RouteNames.Default,
								template: routingSection.Value.DefaultRouteTemplate,
								defaults: new { },
								constraints: new
								{
									area = $@"^{routingSection.Value.BlogAreas}",
									culture = $@"^{blogCultures.SupportedCulturesDelimitedString}",
									id = @"\d+"
								},
								dataTokens: new
								{
									RouteName = RouteNames.Default
								}
						 );

				routes.MapRoute
						 (
								name: RouteNames.Redirect,
								template: routingSection.Value.RedirectRouteTemplate,
								defaults: new
								{
									area = RouteNames.Redirect,
									controller = RouteNames.Redirect,
									action = RouteNames.Redirect
								},
								constraints: new { },
								dataTokens: new
								{
									RouteName = RouteNames.Default
								}
						 );
			});
		}
	}
}
