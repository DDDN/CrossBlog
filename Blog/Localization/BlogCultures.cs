/*
* DDDN.CrossBlog.Blog.Localization.BlogCultures
* 
* Copyright(C) 2017 Lukasz Jaskiewicz
* Author: Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
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

using DDDN.CrossBlog.Blog.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace DDDN.CrossBlog.Blog.Localization
{
	public class BlogCultures : IBlogCultures, IEnumerable<KeyValuePair<CultureInfo, bool>>
	{
		public CultureInfo DefaultCulture { get; protected set; }
		public string SupportedCulturesDelimitedString { get; protected set; }
		private IList<KeyValuePair<CultureInfo, bool>> SupportedCultures { get; set; }
		private LocalizationConfigSection _localizationConfig;

		public BlogCultures(IOptions<LocalizationConfigSection> localizationConfig)
		{
			_localizationConfig = localizationConfig.Value;

			var supportedCulturesDelimitedConfigString = _localizationConfig.SupportedCultures.Trim();
			SupportedCulturesDelimitedString = supportedCulturesDelimitedConfigString.Replace
				 (
					  _localizationConfig.DefaultForNaturalCulturesSuffix, "",
					  StringComparison.InvariantCultureIgnoreCase
				 );

			DefaultCulture = new CultureInfo(_localizationConfig.DefaultCulture.Trim());

			SupportedCultures = new List<KeyValuePair<CultureInfo, bool>>();

			foreach (string cult in supportedCulturesDelimitedConfigString.Split('|'))
			{
				var isDefaultForNaturalCult = default(bool);
				var blogCult = default(string);

				if (cult.EndsWith(_localizationConfig.DefaultForNaturalCulturesSuffix))
				{
					isDefaultForNaturalCult = true;
					blogCult = cult.TrimEnd(_localizationConfig.DefaultForNaturalCulturesSuffix.ToCharArray());
				}
				else
				{
					isDefaultForNaturalCult = false;
					blogCult = cult;
				}

				SupportedCultures.Add(new KeyValuePair<CultureInfo, bool>(new CultureInfo(blogCult), isDefaultForNaturalCult));
			}
		}

		public IEnumerator<KeyValuePair<CultureInfo, bool>> GetEnumerator()
		{
			return SupportedCultures.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return SupportedCultures.GetEnumerator();
		}
	}
}
