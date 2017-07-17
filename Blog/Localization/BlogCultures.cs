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
