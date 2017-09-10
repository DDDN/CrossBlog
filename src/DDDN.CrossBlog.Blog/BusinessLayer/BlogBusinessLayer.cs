/*
DDDN.CrossBlog.Blog.BusinessLayer.BlogBusinessLayer
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Data;
using DDDN.CrossBlog.Blog.Exceptions;
using DDDN.CrossBlog.Blog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.BusinessLayer
{
	public class BlogBusinessLayer : IBlogBusinessLayer
	{
		private CrossBlogContext _context;

		public BlogBusinessLayer(CrossBlogContext context)
		{
			_context = context ?? throw new System.ArgumentNullException(nameof(context));
		}

		public async Task<BlogModel> DetailsGet()
		{
			return await _context.Blog
				.AsNoTracking()
				.FirstAsync();
		}

		public async Task<BlogViewModel> EditGet()
		{
			var blog = await _context.Blog
				.AsNoTracking()
				.FirstAsync();

			var blogView = new BlogViewModel
			{
				BlogId = blog.BlogId,
				BlogName = blog.Name,
				BlogCopyright = blog.Copyright
			};

			return blogView;
		}

		public async Task EditSave(BlogViewModel blogViewModel)
		{
			if (blogViewModel == null)
			{
				throw new ArgumentNullException(nameof(blogViewModel));
			}

			var blog = await _context.Blog
				.Where(p => p.BlogId.Equals(blogViewModel.BlogId))
				.FirstAsync();

			if (blog == default(BlogModel))
			{
				throw new BlogNotFoundException(blogViewModel.BlogId);
			}

			blog.Name = blogViewModel?.BlogName;
			blog.Copyright = blogViewModel?.BlogCopyright;

			await _context.SaveChangesAsync();
		}
	}
}
