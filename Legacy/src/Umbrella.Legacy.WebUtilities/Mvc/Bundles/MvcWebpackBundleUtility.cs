using Microsoft.Extensions.Logging;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.WebUtilities.Bundling.Abstractions;
using Umbrella.WebUtilities.Bundling.Options;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles
{
	public class MvcWebpackBundleUtility : MvcBundleUtility<IWebpackBundleUtility, WebpackBundleUtilityOptions>, IMvcWebpackBundleUtility
	{
		public MvcWebpackBundleUtility(
			ILogger<MvcBundleUtility> logger,
			IHybridCache hybridCache,
			ICacheKeyUtility cacheKeyUtility,
			IWebpackBundleUtility bundleUtility,
			WebpackBundleUtilityOptions options)
			: base(logger, hybridCache, cacheKeyUtility, bundleUtility, options)
		{
		}
	}
}