/*
DDDN.CrossBlog.Blog.Controllers.CrossBlogController
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;

namespace DDDN.CrossBlog.Blog.Controllers
{
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class CrossBlogController : Controller
	{
		protected IActionResult RedirectToLocal(string returnUrl, string defaultController, string defaultAction)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction(defaultController, defaultAction);
			}
		}
	}
}