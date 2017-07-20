using System.Collections.Generic;
using System.Globalization;

namespace DDDN.CrossBlog.Blog.Localization
{
	public interface IBlogCultures : IEnumerable<KeyValuePair<CultureInfo, bool>>
	{
		CultureInfo DefaultCulture { get; }
		string SupportedCulturesDelimitedString { get; }
	}
}