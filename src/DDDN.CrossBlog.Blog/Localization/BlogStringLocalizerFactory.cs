/*
DDDN.CrossBlog.Blog.Localization.BlogStringLocalizerFactory
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace DDDN.CrossBlog.Blog.Localization
{
	public class BlogStringLocalizerFactory : IStringLocalizerFactory
	{
		private readonly IOptions<LocalizationConfigSection> _localizationConfigSection;
		private readonly IHostingEnvironment _hostingEnvironment;
		private readonly ConcurrentDictionary<string, IStringLocalizer> _resourceLocalizations = new ConcurrentDictionary<string, IStringLocalizer>();


		public BlogStringLocalizerFactory(IHostingEnvironment hostingEnvironment, IOptions<LocalizationConfigSection> localizationConfigSection)
		{
			_hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
			_localizationConfigSection = localizationConfigSection ?? throw new ArgumentNullException(nameof(localizationConfigSection));
		}

		public IStringLocalizer Create(Type resourceSource)
		{
			string resourceName = IgnoreOfficeLocalizerFactoryTypeNamespaceLocation(resourceSource.FullName);

			if (_resourceLocalizations.Keys.Contains(resourceName))
			{
				return _resourceLocalizations[resourceName];
			}

			return GetLocalizer(resourceName);
		}

		private string IgnoreOfficeLocalizerFactoryTypeNamespaceLocation(string resourceSourceFullName)
		{
			if (resourceSourceFullName == null)
			{
				throw new ArgumentNullException(nameof(resourceSourceFullName));
			}

			if (_localizationConfigSection.Value.IgnoreOfficeLocalizerFactoryTypeNamespaceLocation
				 & !string.IsNullOrWhiteSpace(_localizationConfigSection.Value.OfficeLocalizerFactoryTypeNamespaceLocation))
			{
				return resourceSourceFullName.Replace(
					 _localizationConfigSection.Value.OfficeLocalizerFactoryTypeNamespaceLocation + ".",
					 "",
					 StringComparison.InvariantCultureIgnoreCase);
			}

			return resourceSourceFullName;
		}

		public IStringLocalizer Create(string baseName, string location)
		{
			string resourceName = IgnoreOfficeLocalizerFactoryTypeNamespaceLocation(baseName);

			if (_resourceLocalizations.Keys.Contains(resourceName))
			{
				return _resourceLocalizations[resourceName];
			}

			return GetLocalizer(resourceName);
		}

		private IStringLocalizer GetLocalizer(string resourceName)
		{
			var officeStringLocalizer = new BlogStringLocalizer(GetAllFromOfficeFile(resourceName), resourceName);
			return _resourceLocalizations.GetOrAdd(resourceName, officeStringLocalizer);
		}

		private Dictionary<string, string> GetAllFromOfficeFile(string resourceKey)
		{
			var resourcePath = Path.Combine(_hostingEnvironment.WebRootPath, _localizationConfigSection.Value.WwwrootL10nFolder);
			IODTStringResource stringRes = new ODTStringResource(resourceKey, resourcePath);
			var translations = stringRes.GetStrings();
			return translations;
		}
	}
}
