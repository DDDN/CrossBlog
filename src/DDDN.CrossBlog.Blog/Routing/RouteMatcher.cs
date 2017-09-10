/*
DDDN.CrossBlog.Blog.Routing.RouteMatcher
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace DDDN.CrossBlog.Blog.Routing
{
	public class RouteMatcher : IRouteMatcher
	{
		public RouteValueDictionary Match(string routeTemplate, string requestPath)
		{
			if (string.IsNullOrWhiteSpace(routeTemplate))
			{
				throw new System.ArgumentException(nameof(routeTemplate));
			}

			if (string.IsNullOrWhiteSpace(requestPath))
			{
				throw new System.ArgumentException(nameof(requestPath));
			}

			var template = TemplateParser.Parse(routeTemplate);
			var matcher = new TemplateMatcher(template, GetDefaults(template));
			var routeValues = new RouteValueDictionary();
			matcher.TryMatch(requestPath, routeValues);

			return routeValues;
		}

		private RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
		{
			if (parsedTemplate == null)
			{
				throw new System.ArgumentNullException(nameof(parsedTemplate));
			}

			var routeValues = new RouteValueDictionary();

			foreach (var parameter in parsedTemplate.Parameters)
			{
				if (parameter.DefaultValue != null)
				{
					routeValues.Add(parameter.Name, parameter.DefaultValue);
				}
			}

			return routeValues;
		}
	}
}
