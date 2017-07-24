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
            services.AddOptions();
            services.Configure<LocalizationConfigSection>(Config.GetSection(ConfigSectionNames.Localization));
            services.Configure<LocalizationConfigSection>(Config.GetSection(ConfigSectionNames.Routing));
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<LocalizationConfigSection>>().Value);
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<RoutingConfigSection>>().Value);
            services.TryAddSingleton<IStringLocalizerFactory, OfficeStringLocalizerFactory>();
            services.TryAddSingleton<IStringLocalizer, OfficeStringLocalizer>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IBlogCultures, BlogCultures>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
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
