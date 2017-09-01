/*
* DDDN.CrossBlog.Blog.Areas.Administration.Controllers.CategoriesController
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

using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.Areas.Administration.Controllers
{
	[Area("Administration")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class CategoriesController : Controller
	{
		private readonly CrossBlogContext _cxt;

		public CategoriesController(CrossBlogContext context)
		{
			_cxt = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IActionResult> Index()
		{
			var categoryNames = await _cxt.Categories.AsNoTracking().ToListAsync();
			return View(categoryNames);
		}

		[HttpPost]
		public async Task<IActionResult> Create(string Name)
		{
			if (!string.IsNullOrWhiteSpace(Name))
			{
				var category = new Category
				{
					CategoryId = Guid.NewGuid(),
					Name = Name
				};

				_cxt.Categories.Add(category);
				await _cxt.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> Delete(Guid CategoryId)
		{
			var category = await _cxt.Categories.Where(p => p.CategoryId.Equals(CategoryId)).FirstOrDefaultAsync();

			if (category != default(Category))
			{
				_cxt.Remove(category);
				await _cxt.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
