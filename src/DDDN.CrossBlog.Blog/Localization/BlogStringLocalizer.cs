/*
DDDN.CrossBlog.Blog.Localization.BlogStringLocalizer
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DDDN.CrossBlog.Blog.Localization
{
	public class BlogStringLocalizer : IStringLocalizer
	{
		private readonly Dictionary<string, string> _localizations;
		private readonly string _resourceKey;

		public BlogStringLocalizer(Dictionary<string, string> localizations, string resourceKey)
		{
			_localizations = localizations;
			_resourceKey = resourceKey;
		}
		public LocalizedString this[string name]
		{
			get
			{
				return new LocalizedString(name, GetText(name));
			}
		}

		public LocalizedString this[string name, params object[] arguments]
		{
			get
			{
				return new LocalizedString(name, GetText(name));
			}
		}

		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			throw new NotImplementedException();
		}

		public IStringLocalizer WithCulture(CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private string GetText(string key)
		{

#if NET451
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
#elif NET46
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
#else
			var culture = CultureInfo.CurrentCulture.ToString();
#endif
			string computedKey = $"{key}.{culture}";

			if (_localizations.TryGetValue(computedKey, out string result))
			{
				return result;
			}
			else
			{
				return _resourceKey + "." + computedKey;
			}
		}
	}
}
