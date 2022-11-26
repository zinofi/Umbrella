using Microsoft.Extensions.Logging;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.WebUtilities.Bundling.Abstractions;
using Umbrella.WebUtilities.Bundling.Options;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles;

/// <summary>
/// A utility that can generate HTML script and style/link tags for embedding named Webpack bundles inside a HTML document.
/// </summary>
/// <seealso cref="MvcBundleUtility{IWebpackBundleUtility, WebpackBundleUtilityOptions}" />
/// <seealso cref="IMvcWebpackBundleUtility" />
public class MvcWebpackBundleUtility : MvcBundleUtility<IWebpackBundleUtility, WebpackBundleUtilityOptions>, IMvcWebpackBundleUtility
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MvcWebpackBundleUtility"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="hybridCache">The hybrid cache.</param>
	/// <param name="cacheKeyUtility">The cache key utility.</param>
	/// <param name="bundleUtility">The bundle utility.</param>
	/// <param name="options">The options.</param>
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