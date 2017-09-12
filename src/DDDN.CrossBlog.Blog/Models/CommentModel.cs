/*
DDDN.CrossBlog.Blog.Models.CommentModel
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

namespace DDDN.CrossBlog.Blog.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("Comment")]
	public class CommentModel
	{
		public enum States
		{
			None,
			Visible,
			Invisible
		}

		[Key]
		public Guid CommentId { get; set; }
		public Guid? ParentId { get; set; }
		[Required]
		public States State { get; set; }
		[Required]
		public DateTimeOffset Created { get; set; }
		[StringLength(100)]
		[Required]
		public string Name { get; set; }
		[StringLength(100)]
		public string Title { get; set; }
		[Required]
		public string Text { get; set; }

		public PostModel Post { get; set; }
		public CommentModel Parent { get; set; }
		public List<CommentModel> Children { get; set; }
	}
}
