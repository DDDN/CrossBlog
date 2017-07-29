using DDDN.CrossBlog.Blog.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DDDN.CrossBlog.Blog.Localization
{
    public class BlogStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IOptions<LocalizationConfigSection> _localizationConfigSection;
        private readonly ConcurrentDictionary<string, IStringLocalizer> _resourceLocalizations = new ConcurrentDictionary<string, IStringLocalizer>();


        public BlogStringLocalizerFactory(IOptions<LocalizationConfigSection> localizationConfigSection)
        {
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
            var dict = new Dictionary<string, string>();
            dict.Add("Test1.pl-PL", "Test1.pl-PLv");
            dict.Add("Test1.en-US", "Test1.pl-PLv");
            dict.Add("Test1.de-DE", "Test1.pl-PLv");
            return dict;
        }
    }
}
