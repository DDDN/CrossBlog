/*
DDDN.CrossBlog.Blog.Models.WriterModel
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

	[Table("Writer")]
	public class WriterModel
	{
		public enum States
		{
			None,
			Active,
			Inactive
		}

		public static Dictionary<string, List<string>> StatesTree = new Dictionary<string, List<string>>
		{
			[nameof(States.Active)] = new List<string> { nameof(States.Active), nameof(States.Inactive) },
			[nameof(States.Inactive)] = new List<string> { nameof(States.Inactive), nameof(States.Active) }
		};

		[Key]
		public Guid WriterId { get; set; }
		[Required]
		public States State { get; set; }
		[Required]
		public DateTimeOffset Created { get; set; }
		[StringLength(256)]
		[Required]
		public string Name { get; set; }
		[StringLength(256)]
		[Required]
		public string Mail { get; set; }
		[Required]
		public bool EmailConfirmed { get; set; }
		[Required]
		public byte[] PasswordHash { get; set; }
		[Required]
		public byte[] Salt { get; set; }

		public BlogModel Blog { get; set; }
		public List<RoleModel> Roles { get; set; }
		public List<PostModel> Posts { get; set; }
	}
}
