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
using DDDN.CrossBlog.Blog.Views.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.BusinessLayer
{
	public class CategoryBusinessLayer : ICategoryBusinessLayer
	{
		private CrossBlogContext _context;

		public CategoryBusinessLayer(CrossBlogContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}
		/// <summary>
		/// Returns a list of categories that are assigned to published postsand the assigned post count for each category.
		/// </summary>
		/// <returns>Returns a list of <list type="CategoryListViewModel"/> objects.</returns>
		public async Task<IEnumerable<CategoryPostCountViewModel>> GetCategoryNamesListAndPublishedPostsCountsOrderdByCategoryName()
		{
			var categories = await _context.PostCategories
				.Where(p => p.Post.State.Equals(PostModel.States.Published))
				.GroupBy(p => p.CategoryId)
				.Select(p => new CategoryPostCountViewModel
				{
					CategoryId = p.Key,
					Name = _context.Categories.First(c => c.CategoryId.Equals(p.Key)).Name,
					Count = p.Count()
				})
				.OrderBy(p => p.Name)
				.ToListAsync();

			return categories;
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
