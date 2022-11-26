using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.Bundling.Abstractions;

/// <summary>
/// A utility for resolving named CSS or JS bundles or relative paths to such bundles.
/// </summary>
public interface IBundleUtility
{
	/// <summary>
	/// Gets the path to the named script bundle or path.
	/// </summary>
	/// <param name="bundleNameOrPath">The bundle name or path.</param>
	/// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
	/// <returns>The application relative path to the bundle.</returns>
	Task<string> GetScriptPathAsync(string bundleNameOrPath, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the script content at the named bundle or path.
	/// </summary>
	/// <param name="bundleNameOrPath">The bundle name or path.</param>
	/// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
	/// <returns>The bundle content.</returns>
	Task<string?> GetScriptContentAsync(string bundleNameOrPath, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the path to the named css bundle or path.
	/// </summary>
	/// <param name="bundleNameOrPath">The bundle name or path.</param>
	/// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
	/// <returns>The application relative path to the bundle.</returns>
	Task<string> GetStyleSheetPathAsync(string bundleNameOrPath, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the css content at the named bundle or path.
	/// </summary>
	/// <param name="bundleNameOrPath">The bundle name or path.</param>
	/// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
	/// <returns>The bundle content.</returns>
	Task<string?> GetStyleSheetContentAsync(string bundleNameOrPath, CancellationToken cancellationToken = default);
}