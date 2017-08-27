/*
* DDDN.CrossBlog.Blog.Views.Models.ViewModel
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

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace DDDN.CrossBlog.Blog.Views.Models
{
	public class ViewModel
	{
		public List<SelectListItem> States { get; }

		public ViewModel(Type enumType, IStringLocalizer localizer)
		{
			States = new List<SelectListItem>();

			if (localizer == null)
			{
				return;
			}

			var values = Enum.GetValues(enumType);
			var names = Enum.GetNames(enumType);

			for (int i = 0; i < values.Length; i++)
			{
				var item = new SelectListItem
				{
					Value = values.GetValue(i) as string,
					Text = localizer[names.GetValue(i) as string]
				};

				States.Add(item);
			}
		}

	}
}
