/*
DDDN.CrossBlog.Blog.Startup
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.BusinessLayer;
using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Data;
using DDDN.CrossBlog.Blog.Localization;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using System;

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

			var authenticationConfigSection = Config.GetSection(ConfigSectionNames.Authentication);
			var sessionDurationInMinutes = authenticationConfigSection.Get<AuthenticationConfigSection>().SessionDurationInMinutes;

			services.AddOptions();
			services.Configure<LocalizationConfigSection>(localizationConfigSection);
			services.AddScoped(cfg => cfg.GetService<IOptions<LocalizationConfigSection>>().Value);

			services.Configure<RoutingConfigSection>(Config.GetSection(ConfigSectionNames.Routing));
			services.AddScoped(cfg => cfg.GetService<IOptions<RoutingConfigSection>>().Value);

			services.Configure<AuthenticationConfigSection>(authenticationConfigSection);
			services.AddScoped(cfg => cfg.GetService<IOptions<AuthenticationConfigSection>>().Value);

			services.Configure<SeedConfigSection>(Config.GetSection(ConfigSectionNames.Seed));
			services.AddScoped(cfg => cfg.GetService<IOptions<SeedConfigSection>>().Value);

			services.TryAddSingleton<IStringLocalizerFactory, BlogStringLocalizerFactory>();
			services.TryAddSingleton<IStringLocalizer, BlogStringLocalizer>();
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.TryAddSingleton<IBlogCultures, BlogCultures>();
			services.TryAddScoped<IBlogBusinessLayer, BlogBusinessLayer>();
			services.TryAddScoped<IPostBusinessLayer, PostBusinessLayer>();
			services.TryAddScoped<IWriterBusinessLayer, WriterBusinessLayer>();
			services.AddLocalization(options => options.ResourcesPath = wwwrootL10nFolder);
			services.AddRouting(options => options.LowercaseUrls = false);

			services.AddAuthentication(options =>
			{
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			}).AddCookie(options =>
			{
				//options.AccessDeniedPath = "/Writers/Forbidden/";
				options.LoginPath = "/Dashboard/Login/";
				options.LogoutPath = "/Dashboard/Logout/";
				options.ExpireTimeSpan = new TimeSpan(0, sessionDurationInMinutes, 0);
				options.Cookie = new CookieBuilder
				{
					SameSite = SameSiteMode.Lax,
					Expiration = new TimeSpan(0, sessionDurationInMinutes, 0)
				};
			}); ;

			services.AddMvc()
				 .AddViewLocalization();

			services.AddDbContext<CrossBlogContext>(options =>
				options.UseSqlServer(Config.GetConnectionString("CrossBlogLocalDBConnection")));

			services.AddTransient<CrossBlogContextInitializer>();
		}

		public void Configure(
			  IApplicationBuilder app,
			  IHostingEnvironment env,
			  IBlogCultures blogCultures,
			  CrossBlogContextInitializer crossBlogContextInitializer,
			  IOptions<RoutingConfigSection> routingSection)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();

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

			crossBlogContextInitializer.Initialize();
		}
	}
}
