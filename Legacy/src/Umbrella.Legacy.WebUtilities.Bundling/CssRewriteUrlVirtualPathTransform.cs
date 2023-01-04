using System.Web;
using System.Web.Optimization;

namespace Umbrella.Legacy.WebUtilities.Bundling;

/// <summary>
/// An <see cref="IItemTransform"/> implementation that rewrites virtual paths inside CSS files to absolute paths relative to the application.
/// </summary>
/// <seealso cref="IItemTransform" />
public class CssRewriteUrlVirtualPathTransform : IItemTransform
{
	/// <inheritdoc />
	public string Process(string includedVirtualPath, string input)
		=> new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath).ToLowerInvariant(), input);
}