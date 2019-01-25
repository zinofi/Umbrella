using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Caching
{
    public enum MultiCacheAnalyticsMode
    {
        None = 0,
        TrackKeys = 1,
        TrackKeysAndHits = 3
    }
}