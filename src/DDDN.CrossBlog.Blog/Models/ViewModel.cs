/*
DDDN.CrossBlog.Blog.Views.Models.ViewModel
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

namespace DDDN.CrossBlog.Blog.Views.Models
{
	public class ViewModel
	{
		public List<SelectListItem> States { get; }

		public ViewModel(IEnumerable<string> states, IStringLocalizer localizer)
		{
			if (states == null)
			{
				throw new System.ArgumentNullException(nameof(states));
			}

			States = new List<SelectListItem>();

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
