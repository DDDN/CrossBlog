/*
DDDN.CrossBlog.Blog.Data.CrossBlogInitializer
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Exceptions;
using System;
using System.Linq;

namespace DDDN.CrossBlog.Blog.Data
{
	public class CrossBlogInitializer
	{
		private readonly CrossBlogContext _context;
		private readonly BlogInfo _blogInfo;

		public CrossBlogInitializer(
			CrossBlogContext context,
			BlogInfo blogInfo)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_blogInfo = blogInfo ?? throw new ArgumentNullException(nameof(blogInfo));
		}

		public void Initialize()
		{
			InitBlogInfo();
		}

		private void InitBlogInfo()
		{
			var blogs = _context.Blog.ToList();

			if (blogs.Count != 1)
			{
				throw new CrossBlogException("There should be only one BlogModel entity.");
			}

			var blog = blogs.First();
			var owner = _context.Writers.FirstOrDefault(p => p.WriterId.Equals(blog.OwnerId));

			_blogInfo.BlogName = blog.Name;
			_blogInfo.Version = blog.Version;
			_blogInfo.OwnerId = owner.WriterId;
			_blogInfo.OwnerName = owner.Name;
			_blogInfo.OwnerMail = owner.Mail;
		}
	}
}
