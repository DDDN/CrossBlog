/*
DDDN.CrossBlog.Blog.Routing.BlogRequestCultureProvider
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DDDN.CrossBlog.Blog.Routing
{
    public class BlogRequestCultureProvider : RequestCultureProvider
	{
		public string RouteDataStringKey { get; set; } = "culture";
		private new static readonly Task<ProviderCultureResult> NullProviderCultureResult = Task.FromResult(default(ProviderCultureResult));

		public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException(nameof(httpContext));
			}

			var blogCultures = httpContext.RequestServices.GetService<IBlogCultures>();
			var routingConfigSection = httpContext.RequestServices.GetService<IOptions<RoutingConfigSection>>();

			if (!string.IsNullOrWhiteSpace(routingConfigSection.Value.CultureRouteDataStringKey))
			{
				RouteDataStringKey = routingConfigSection.Value.CultureRouteDataStringKey;
			}

			if (blogCultures == null)
			{
				throw new NullReferenceException(nameof(blogCultures));
			}

			string cultureName = GetCultureNameFromRoute(
				 httpContext.Request.Path,
				 routingConfigSection.Value.DefaultRouteTemplate,
				 routingConfigSection.Value.CultureRouteDataStringKey);
			var cultureInfo = GetCulture(cultureName, httpContext, blogCultures);
			var providerResultCulture = new ProviderCultureResult(cultureInfo.Name, cultureInfo.Name);

			return Task.FromResult(providerResultCulture);
		}

		public static string GetCultureNameFromRoute(string requestPath, string routeTemplate, string routeDataStringKey)
		{
			if (string.IsNullOrWhiteSpace(requestPath))
			{
				throw new ArgumentException(nameof(requestPath));
			}

			if (string.IsNullOrWhiteSpace(routeTemplate))
			{
				throw new ArgumentException(nameof(routeTemplate));
			}

			if (string.IsNullOrWhiteSpace(routeDataStringKey))
			{
				throw new ArgumentException(nameof(routeDataStringKey));
			}

			var defaultRoute = new RouteMatcher().Match(routeTemplate, requestPath);
			var cultureName = defaultRoute[routeDataStringKey] as string;
			return cultureName;
		}

		public static CultureInfo GetCulture(string routeCultureName, HttpContext httpContext, IBlogCultures blogCultures)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException(nameof(httpContext));
			}

			if (blogCultures == null)
			{
				throw new ArgumentNullException(nameof(blogCultures));
			}

			if (TryCreateCultureInfo(routeCultureName, out CultureInfo blogCultureInfo))
			{
				var routeCultures = new List<CultureInfo>() { blogCultureInfo };
				blogCultureInfo = FindCulture(routeCultures, blogCultures);

				if (blogCultureInfo != null)
				{
					return blogCultureInfo;
				}
			}

			var acceptLanguageHeaderValue = httpContext.Request.Headers[HeaderNames.AcceptLanguage].FirstOrDefault<string>();

			if (acceptLanguageHeaderValue != default(string))
			{
				var headerCultures = acceptLanguageHeaderValue.Split(',')
					 .Select(System.Net.Http.Headers.StringWithQualityHeaderValue.Parse)
					 .OrderByDescending(s => s.Quality.GetValueOrDefault(1)).Select(s => new CultureInfo(s.Value)).ToList();

				if (headerCultures.Any())
				{
					blogCultureInfo = FindCulture(headerCultures, blogCultures);
				}
			}

			if (blogCultureInfo == null)
			{
				blogCultureInfo = blogCultures.DefaultCulture;
			}

			return blogCultureInfo;
		}

		public static bool TryCreateCultureInfo(string routeCultureName, out CultureInfo cultureInfo)
		{
			cultureInfo = null;

			try
			{
				if (!string.IsNullOrWhiteSpace(routeCultureName))
				{
					cultureInfo = CultureInfo.GetCultureInfo(routeCultureName);
					return true;
				}
			}
			catch (Exception ex) when (ex is CultureNotFoundException || ex is ArgumentNullException)
			{
			}

			return false;
		}

		private static CultureInfo FindCulture(List<CultureInfo> cultures, IEnumerable<KeyValuePair<CultureInfo, bool>> blogCultures)
		{
			if (cultures == null)
			{
				throw new ArgumentNullException(nameof(cultures));
			}

			if (blogCultures == null)
			{
				throw new ArgumentNullException(nameof(blogCultures));
			}

			CultureInfo cultureInfo = default(CultureInfo);

			foreach (var cult in cultures)
			{
				if (cult.IsNeutralCulture)
				{
					cultureInfo = blogCultures
						 .Where(p => p.Key.TwoLetterISOLanguageName.Equals(cult.Name, StringComparison.OrdinalIgnoreCase) && p.Value)
						 .Select(p => p.Key).FirstOrDefault<CultureInfo>();
				}
				else
				{
					cultureInfo = blogCultures
						 .Where(p => p.Key.Name.Equals(cult.Name, StringComparison.OrdinalIgnoreCase))
						 .Select(p => p.Key).FirstOrDefault<CultureInfo>();
				}

				if (cultureInfo != default(CultureInfo))
				{
					break;
				}
			}

			if (cultureInfo == default(CultureInfo))
			{
				foreach (var cult in cultures)
				{
					cultureInfo = blogCultures
						 .Where(p => p.Key.TwoLetterISOLanguageName.Equals(cult.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
						 .Select(p => p.Key).FirstOrDefault<CultureInfo>();

					if (cultureInfo != default(CultureInfo))
					{
						break;
					}
				}
			}

			return cultureInfo;
		}
	}
}