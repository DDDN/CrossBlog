/*
DDDN.CrossBlog.Blog.ViewComponents.CategoriesViewComponent
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.ViewComponents
{
	public class CategoriesViewComponent : ViewComponent
	{
		private CrossBlogContext _cxt;

		public CategoriesViewComponent(CrossBlogContext cxt)
		{
			_cxt = cxt ?? throw new ArgumentNullException(nameof(cxt));
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var categories = await _cxt.Categories
				.Include(p => p.PostCategories).ToListAsync();

			return View(categories);
		}
	}
}
