/*
DDDN.CrossBlog.Blog.Exceptions
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;

namespace DDDN.CrossBlog.Blog.Exceptions
{
	public class PostContentNotFoundException : Exception
	{
		public Guid ContentId { get; private set; }
		public PostContentNotFoundException(string message, Guid contentId) : base(message) { ContentId = contentId; }
		public PostContentNotFoundException(string message, Guid contentId, Exception innerException) : base(message, innerException) { ContentId = contentId; }
		public PostContentNotFoundException(Guid contentId) { ContentId = contentId; }
		public PostContentNotFoundException() { }
		public PostContentNotFoundException(string message) : base(message) { }
		public PostContentNotFoundException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class BlogNotFoundException : Exception
	{
		public Guid BlogId { get; private set; }
		public BlogNotFoundException(string message, Guid blogId) : base(message) { BlogId = blogId; }
		public BlogNotFoundException(string message, Guid blogId, Exception innerException) : base(message, innerException) { BlogId = blogId; }
		public BlogNotFoundException(Guid blogId) { BlogId = blogId; }
		public BlogNotFoundException() { }
		public BlogNotFoundException(string message) : base(message) { }
		public BlogNotFoundException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class PostNotFoundException : Exception
	{
		public Guid PostId { get; private set; }
		public PostNotFoundException(string message, Guid postId) : base(message) { PostId = postId; }
		public PostNotFoundException(string message, Guid postId, Exception innerException) : base(message, innerException) { PostId = postId; }
		public PostNotFoundException(Guid postId) { PostId = postId; }
		public PostNotFoundException() { }
		public PostNotFoundException(string message) : base(message) { }
		public PostNotFoundException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class WriterNotFoundException : Exception
	{
		public Guid WriterId { get; private set; }
		public WriterNotFoundException(string message, Guid writerId) : base(message) { WriterId = writerId; }
		public WriterNotFoundException(string message, Guid writerId, Exception innerException) : base(message, innerException) { WriterId = writerId; }
		public WriterNotFoundException(Guid writerId) { WriterId = writerId; }
		public WriterNotFoundException() { }
		public WriterNotFoundException(string message) : base(message) { }
		public WriterNotFoundException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class CrossBlogException : Exception
	{
		public CrossBlogException() { }
		public CrossBlogException(string message) : base(message) { }
		public CrossBlogException(string message, Exception innerException) : base(message, innerException) { }
	}
}
