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
using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
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

		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			 WebHost.CreateDefaultBuilder(args)
				  .UseStartup<Startup>()
				  .Build();

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
			var wwwrootL10nFolder = localizationConfigSection.Get<LocalizationConfigSection>().WwwrootL10nFolder;

			services.AddOptions();
			services.Configure<LocalizationConfigSection>(localizationConfigSection);
			services.AddScoped(cfg => cfg.GetService<IOptions<LocalizationConfigSection>>().Value);

			services.Configure<RoutingConfigSection>(Config.GetSection(ConfigSectionNames.Routing));
			services.AddScoped(cfg => cfg.GetService<IOptions<RoutingConfigSection>>().Value);

			services.Configure<BlogConfigSection>(Config.GetSection(ConfigSectionNames.Blog));
			services.AddScoped(cfg => cfg.GetService<IOptions<BlogConfigSection>>().Value);

			services.TryAddSingleton<IStringLocalizerFactory, BlogStringLocalizerFactory>();
			services.TryAddSingleton<IStringLocalizer, BlogStringLocalizer>();
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.TryAddSingleton<IBlogCultures, BlogCultures>();
			services.AddLocalization(options => options.ResourcesPath = wwwrootL10nFolder);
			services.AddRouting(options => options.LowercaseUrls = false);
			services.AddMvc()
				 .AddViewLocalization();

			services.AddDbContext<CrossBlogContext>(options =>
				options.UseSqlServer(Config.GetConnectionString("CrossBlogLocalDBConnection")));
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
					},
					dataTokens: new
					{
						RouteName = RouteNames.Default
					}
				 );

				routes.MapRoute
				(
					name: RouteNames.PostContent,
					template: routingSection.Value.PostContentTemplate,
					defaults: new
					{
						area = routingSection.Value.DefaultArea,
						controller = routingSection.Value.DefaultController,
						action = RouteNames.PostContent
					},
					constraints: new
					{
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
						RouteName = RouteNames.Redirect
					}
				);
			});

			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var cxt = serviceScope.ServiceProvider.GetService<CrossBlogContext>();
				//.Database.Migrate();
				CrossBlogContextInitializer.Initialize(cxt);
			}
		}
	}
}
