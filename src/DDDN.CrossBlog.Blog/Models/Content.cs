/*
* DDDN.CrossBlog.Blog.Models.Content
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

namespace DDDN.CrossBlog.Blog.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("Content")]
	public class Content
	{
		public enum States
		{
			Visible,
			Unvisible
		}

		public static Dictionary<string, List<string>> StatesTree = new Dictionary<string, List<string>>
		{
			[nameof(States.Visible)] = new List<string> { nameof(States.Visible), nameof(States.Unvisible) },
			[nameof(States.Unvisible)] = new List<string> { nameof(States.Unvisible), nameof(States.Visible) }
		};

		[Key]
		public Guid ContentId { get; set; }
		[Required]
		public States State { get; set; }
		[Required]
		public DateTimeOffset Created { get; set; }
		[Required]
		public byte[] Binary { get; set; }
		[Required]
		public string Name { get; set; }

		public Post Post { get; set; }
	}
}
