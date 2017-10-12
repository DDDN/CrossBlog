/*
DDDN.CrossBlog.Blog.Localization.ODTStringResource
Copyright(C) 2017 Lukasz Jaskiewicz(lukasz @jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.Office.Odf;
using DDDN.Office.Odf.Odt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DDDN.CrossBlog.Blog.Localization
{
	public class ODTStringResource : IODTStringResource
	{
		private string ResourceKey;
		private string ResourceFolderPath;

		public ODTStringResource(string resourceKey, string resourceFolderPath)
		{
			if (string.IsNullOrWhiteSpace(resourceKey))
			{
				throw new ArgumentException(nameof(string.IsNullOrWhiteSpace), nameof(resourceKey));
			}

			if (string.IsNullOrWhiteSpace(resourceFolderPath))
			{
				throw new ArgumentException(nameof(string.IsNullOrWhiteSpace), nameof(resourceFolderPath));
			}

			ResourceKey = resourceKey;
			ResourceFolderPath = resourceFolderPath;
		}

		public Dictionary<string, string> GetStrings()
		{
			var ret = new Dictionary<string, string>();
			List<string> resourcefileFullPaths = GetResourcefileFullPaths();

			foreach (var fileFullPath in resourcefileFullPaths)
			{
				var cultureNameFromFileName = Path.GetFileNameWithoutExtension(fileFullPath).Split('.').Last();

				using (IODTFile odtFile = new ODTFile(fileFullPath))
				{
					var transl = GetTranslations(cultureNameFromFileName, odtFile);
					ret = ret.Union(transl).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
				}
			}

			return ret;
		}

		public static Dictionary<string, string> GetTranslations(string cultureNameFromFileName, IODTFile odtFile)
		{
			var translations = new Dictionary<string, string>();

			XDocument contentXDoc = odtFile.GetZipArchiveEntryAsXDocument("content.xml");

			var contentEle = contentXDoc.Root
				 .Elements(XName.Get("body", ODFXmlNamespaces.Office))
				 .Elements(XName.Get("text", ODFXmlNamespaces.Office))
				 .First();

			foreach (var table in contentEle.Elements()
				 .Where(p => p.Name.LocalName.Equals("table", StringComparison.CurrentCultureIgnoreCase)))
			{
				foreach (var row in table.Elements()
				.Where(p => p.Name.LocalName.Equals("table-row", StringComparison.CurrentCultureIgnoreCase))
				.Skip(1))
				{
					var cells = row.Elements()
						 .Where(p => p.Name.LocalName.Equals("table-cell", StringComparison.CurrentCultureIgnoreCase));

					if (cells.Any())
					{
						var translationKey = ODTReader.GetValue(cells.First());
						var translation = ODTReader.GetValue(cells.Skip(1).First());
						translations.Add($"{translationKey}.{cultureNameFromFileName}", translation);
					}
				}
			}

			return translations;
		}

		private List<string> GetResourcefileFullPaths()
		{
			var lastPointIndex = ResourceKey.LastIndexOf('.');
			var odtFileName = ResourceKey.Substring(lastPointIndex + 1);
			var l10nPath = Path.Combine(
				 ResourceFolderPath,
				 ResourceKey.Remove(lastPointIndex).Replace('.', '\\'),
				 "l10n");
			var fileNames = odtFileName.Split('+');
			List<string> resourcefileFullPaths = new List<string>();

			foreach (var file in fileNames)
			{
				resourcefileFullPaths.AddRange(Directory.GetFiles(l10nPath, $"{file}.*"));
			}

			return resourcefileFullPaths;
		}
	}
}
