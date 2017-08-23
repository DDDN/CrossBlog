/*
* DDDN.CrossBlog.Blog.Model.Data.Writer
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

namespace DDDN.CrossBlog.Blog.Model.Data
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("Writer")]
	public partial class Writer
	{
		[Key]
		public Guid WriterId { get; set; }
		public WriterState State { get; set; }
		public DateTimeOffset Created { get; set; }
		[Required]
		[StringLength(100)]
		public string Name { get; set; }
		[StringLength(100)]
		public string Mail { get; set; }
		public byte[] Password { get; set; }
		public byte[] Salt { get; set; }

		public List<Session> Sessions { get; set; }
		public List<Post> Posts { get; set; }
	}
}
