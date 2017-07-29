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
