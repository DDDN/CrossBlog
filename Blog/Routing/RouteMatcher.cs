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
