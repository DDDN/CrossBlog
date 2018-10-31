/* 
DDDN.CrossBlog.Blog.Data.BusinessLayer.ICategoryBusinessLayer
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Views.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.BusinessLayer
{
	public interface ICategoryBusinessLayer
	{
		Task<IEnumerable<CategoryPostCountViewModel>> GetCategoryNamesListAndPublishedPostsCountsOrderdByCategoryName();
		Task<IEnumerable<CategoryModel>> Get();
		Task Create(string categoryName);
		Task Delete(Guid categoryId);
		Task Rename(Guid categoryId, string categoryName);
	}
}
