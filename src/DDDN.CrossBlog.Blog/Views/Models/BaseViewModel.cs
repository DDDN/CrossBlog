/*
DDDN.CrossBlog.Blog.Views.Models.BaseViewModel
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using static DDDN.CrossBlog.Blog.Views.Models.ViewMessage;

namespace DDDN.CrossBlog.Blog.Views.Models
{
	public class BaseViewModel
	{
		public List<SelectListItem> States { get; set; } = new List<SelectListItem>();
		public List<ViewMessage> Msgs { get; } = new List<ViewMessage>();

		public void AddMsg(string msgText, MsgType msgType)
		{
			Msgs.Add(new ViewMessage(msgText, msgType));
		}

		public BaseViewModel(IEnumerable<string> states = null, IStringLocalizer localizer = null)
		{
			if (states == null)
			{
				return;
			}

			foreach (var state in states)
			{
				var item = new SelectListItem
				{
					Value = state,
					Text = localizer == null ? state : localizer[state]
				};

				States.Add(item);
			}
		}
	}
}
