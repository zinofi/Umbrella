using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Caching.Options
{
    public delegate string MultiCacheKeyBuilder(Type type, string key);

    public class MultiCacheOptions
    {
        public bool CacheEnabled { get; set; }
        public MultiCacheKeyBuilder CacheKeyBuilder { get; set; }
        public TimeSpan MaxCacheTimeout { get; set; } = TimeSpan.FromDays(3650); // Arbitrary default of ~10 years.
        public MultiCacheAnalyticsMode AnalyticsMode { get; set; }
    }
}