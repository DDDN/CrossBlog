/*
DDDN.CrossBlog.Blog.Models.RoleModel
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
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("Role")]
	public class RoleModel
	{
		public enum Roles
		{
			None,
			Administrator,
			Writer
		}

		public static RoleModel Get(Roles role)
		{
			return new RoleModel
			{
				RoleId = Guid.NewGuid(),
				Role = role
			};
		}

		[Key]
		public Guid RoleId { get; set; }
		[Required]
		public Roles Role { get; set; }

		public WriterModel Writer { get; set; }
	}
}
