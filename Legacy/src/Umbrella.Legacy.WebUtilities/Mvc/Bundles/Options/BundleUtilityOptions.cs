using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Abstractions;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles.Options
{
    public class BundleUtilityOptions : CacheableUmbrellaOptions
    {
        public string DefaultBundleFolderAppRelativePath { get; set; }
        public bool WatchFiles { get; set; }
		public bool? AppendVersion { get; set; }
	}
}