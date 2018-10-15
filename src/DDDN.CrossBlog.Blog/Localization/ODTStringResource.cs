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

using DDDN.OdtToHtml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
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
				using (IOdtFile odtFile = new OdtFile(fileFullPath))
				{
					var embedContent = odtFile.GetZipArchiveEntries()
						.FirstOrDefault(p => p.ContentFullName.Equals("content.xml", StringComparison.InvariantCultureIgnoreCase))?.Data;

					var contentXDoc = OdtFile.GetZipArchiveEntryAsXDocument(embedContent);

					var transl = GetTranslations(contentXDoc);
					ret = ret.Union(transl).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
				}
			}

			return ret;
		}

		public static Dictionary<string, string> GetTranslations(XDocument content)
		{
			var translations = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

			if (content == null)
			{
				return translations;
			}

			var contentEle = content.Root
				 .Elements(XName.Get("body", OdtXmlNs.Office))
				 .Elements(XName.Get("text", OdtXmlNs.Office))
				 .First();

			foreach (var table in contentEle.Elements()
				 .Where(p => p.Name.LocalName.Equals("table", StringComparison.CurrentCultureIgnoreCase)))
			{
				var tableRows = table.Elements().Where(p => p.Name.LocalName.Equals("table-row", StringComparison.CurrentCultureIgnoreCase));

				if (tableRows.Count() > 2)
				{
					var cultureNameFromFileName = GetValue(tableRows.FirstOrDefault()?
						.Elements().Where(p => p.Name.LocalName.Equals("table-cell", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());

					if (!String.IsNullOrWhiteSpace(cultureNameFromFileName))
					{
						foreach (var row in tableRows.Skip(2))
						{
							var cells = row.Elements()
								 .Where(p => p.Name.LocalName.Equals("table-cell", StringComparison.CurrentCultureIgnoreCase));

							if (cells.Any())
							{
								var translationKey = GetValue(cells.First());
								var translation = GetValue(cells.Skip(1).First());
								translations.Add($"{translationKey}.{cultureNameFromFileName}", translation);
							}
						}
					}
				}
			}

			return translations;
		}

		private static string GetValue(XElement xElement)
		{
			return WalkTheNodes(xElement.Nodes());
		}

		private static string WalkTheNodes(IEnumerable<XNode> nodes)
		{
			string val = "";

			if (nodes == null)
			{
				return val;
			}

			foreach (var node in nodes)
			{
				if (node.NodeType == XmlNodeType.Text)
				{
					var textNode = node as XText;
					val += textNode.Value;
				}
				else if (node.NodeType == XmlNodeType.Element)
				{
					var elementNode = node as XElement;

					if (elementNode.Name.Equals(XName.Get("s", OdtXmlNs.Text)))
					{
						val += " ";
					}
					else
					{
						val += WalkTheNodes(elementNode.Nodes());
					}
				}
			}

			return val;
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
				if (Directory.Exists(l10nPath))
				{
					var files = Directory.GetFiles(l10nPath, $"{file}.*");
					resourcefileFullPaths.AddRange(files);
				}
			}

			return resourcefileFullPaths;
		}
	}
}
