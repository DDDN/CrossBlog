/* 
DDDN.CrossBlog.Blog.Data.BusinessLayer.CategoryBusinessLayer
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Data;
using DDDN.CrossBlog.Blog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.BusinessLayer
{
	public class CategoryBusinessLayer : ICategoryBusinessLayer
	{
		private CrossBlogContext _context;

		public CategoryBusinessLayer(CrossBlogContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<CategoryModel>> Get()
		{
			var categories = await _context.Categories
				.AsNoTracking()
				.ToListAsync();

			return categories;
		}

		public async Task Create(string categoryName)
		{
			var category = new CategoryModel
			{
				CategoryId = Guid.NewGuid(),
				Name = categoryName
			};

			_context.Categories.Add(category);
			await _context.SaveChangesAsync();
		}

		public async Task Delete(Guid categoryId)
		{
			var category = await _context.Categories
				.FirstOrDefaultAsync(p => p.CategoryId.Equals(categoryId));

			if (category != default(CategoryModel))
			{
				_context.Remove(category);
				await _context.SaveChangesAsync();
			}
		}
	}
}
