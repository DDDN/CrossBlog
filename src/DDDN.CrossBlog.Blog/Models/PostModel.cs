/*
DDDN.CrossBlog.Blog.Models.PostModel
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

	[Table("Post")]
	public class PostModel
	{
		public enum States
		{
         None,
			Uploaded,
			Published,
			Hidden,
			Removed
		}

		public static Dictionary<string, List<string>> StatesTree = new Dictionary<string, List<string>>
		{
			[nameof(States.Uploaded)] = new List<string> { nameof(States.Uploaded), nameof(States.Published), nameof(States.Hidden), nameof(States.Removed) },
			[nameof(States.Published)] = new List<string> { nameof(States.Published), nameof(States.Hidden), nameof(States.Removed) },
			[nameof(States.Hidden)] = new List<string> { nameof(States.Hidden), nameof(States.Published), nameof(States.Removed) },
			[nameof(States.Removed)] = new List<string> { nameof(States.Removed) }
		};

		[Key]
		public Guid PostId { get; set; }
		[Required]
		public States State { get; set; }
		[Required]
		public DateTimeOffset Created { get; set; }
		[Required]
		public byte[] Binary { get; set; }
		[Required]
		public byte[] Hash { get; set; }
		[Required]
		public string Html { get; set; }
		[Required]
		public string Css { get; set; }
		[StringLength(200)]
		public string PageCssClassName { get; set; }
		public string FirstHeader { get; set; }
		public string FirstParagraph { get; set; }
		[StringLength(200)]
		public string AlternativeTitle { get; set; }
		[StringLength(500)]
		public string AlternativeTeaser { get; set; }
		[Required]
		public DateTimeOffset LastRenderd { get; set; }
		public DateTimeOffset FirstPublished { get; set; }
		public DateTimeOffset LastPublished { get; set; }

		public WriterModel Writer { get; set; }
		public List<ContentModel> Contents { get; set; }
		public List<CommentModel> Comments { get; set; }
		public List<PostCategoryMap> PostCategories { get; set; }
	}
}
