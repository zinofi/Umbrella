using System.Collections.Generic;
using System.Web.Optimization;

namespace Umbrella.Legacy.WebUtilities.Bundling
{
    public class AsIsBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            => files;
    }
}