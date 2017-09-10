/*
DDDN.CrossBlog.Blog.Configuration.RoutingConfigSection
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

namespace DDDN.CrossBlog.Blog.Configuration
{
	public class RoutingConfigSection
	{
		public string DefaultRouteTemplate { get; set; } = "{culture}/{controller}/{action}/{id?}";
		public string PostContentTemplate { get; set; } = "PostContent/{id}/{filename}";
		public string BlogPostHtmlUrlPrefix { get; set; } = "/PostContent";
		public string RedirectRouteTemplate { get; set; } = "{*url}";
		public string DefaultController { get; set; } = "Blog";
		public string DefaultAction { get; set; } = "Newest";
		public string CultureRouteDataStringKey { get; set; } = "culture";
		public string ReturnUrl { get; set; } = "ReturnUrl";

	}
}
