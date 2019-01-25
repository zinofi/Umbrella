using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles.Options
{
    public class BundleUtilityOptions
    {
        public string DefaultBundleFolderAppRelativePath { get; set; }
        public bool CacheEnabled { get; set; } = true;
        public TimeSpan CacheTimeout { get; set; } = TimeSpan.FromHours(1);
        public bool CacheSlidingExpiration { get; set; } = true;
        public bool WatchFiles { get; set; }
    }
}