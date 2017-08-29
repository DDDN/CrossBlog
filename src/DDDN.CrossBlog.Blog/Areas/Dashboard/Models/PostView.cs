/*
* DDDN.CrossBlog.Blog.Areas.Dashboard.Models.PostView
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

using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Views.Models;
using Microsoft.Extensions.Localization;
using System;

namespace DDDN.CrossBlog.Blog.Areas.Dashboard.Models
{
	public class PostView : ViewModel
	{
		public PostView()
			: base(typeof(Post.States), null)
		{
		}

		public PostView(IStringLocalizer localizer)
			: base(typeof(Post.States), localizer)
		{
		}

		public Guid PostId { get; set; }
		public Post.States State { get; set; }
		public DateTimeOffset Created { get; set; }
		public string FileName { get; set; }
		public string Title { get; set; }
		public string FirstHeaderText { get; set; }
		public string FirstParagraphHtml { get; set; }
		public string AlternativeTitle { get; set; }
		public string AlternativeTeaser { get; set; }
	}
}
