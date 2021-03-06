﻿/*
DDDN.CrossBlog.Blog.Views.Models.WriterViewModel
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Models;
using Microsoft.Extensions.Localization;
using System;

namespace DDDN.CrossBlog.Blog.Views.Models
{
	public class WriterViewModel : BaseViewModel
	{
		public WriterViewModel() : base()
		{
		}

		public WriterViewModel(WriterModel.States state = WriterModel.States.None, IStringLocalizer localizer = null)
			: base(WriterModel.StatesTree[state.ToString()], localizer)
		{
		}

		public WriterViewModel(WriterModel writerModel, bool isAdministrator, IStringLocalizer localizer = null)
			: base(WriterModel.StatesTree[writerModel.State.ToString()], localizer)
		{
			WriterId = writerModel.WriterId;
			State = writerModel.State;
			Mail = writerModel.Mail;
			Name = writerModel.Name;
			AboutMe = writerModel.AboutMe;
			Created = writerModel.Created;
			Administrator = isAdministrator;
		}

		public Guid WriterId { get; set; }
		public WriterModel.States State { get; set; }
		public string Name { get; set; }
		public string Mail { get; set; }
		public string AboutMe { get; set; }
		public bool Administrator { get; set; }
		public string Password { get; set; }
		public string PasswordCompare { get; set; }
		public DateTimeOffset Created { get; set; }
	}
}
