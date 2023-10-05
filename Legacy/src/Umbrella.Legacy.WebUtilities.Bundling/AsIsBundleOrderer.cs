using System.Web.Optimization;

namespace Umbrella.Legacy.WebUtilities.Bundling;

/// <summary>
/// A bundle orderer that doesn't change the order in which files are rendered, i.e. it renders them exactly as registered
/// with the bundling mechanism.
/// </summary>
/// <seealso cref="IBundleOrderer" />
public class AsIsBundleOrderer : IBundleOrderer
{
	/// <inheritdoc />
	public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
		=> files;
}