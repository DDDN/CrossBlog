using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.Configuration
{
    public class LocalizationSection
    {
        public LocalizationSection()
        {
        }

        public string DefaultCulture { get; set; }
        public string SupportedCultures { get; set; }
    }
}
