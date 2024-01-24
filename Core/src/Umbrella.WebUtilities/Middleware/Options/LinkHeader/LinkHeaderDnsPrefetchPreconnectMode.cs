namespace Umbrella.WebUtilities.Middleware.Options.LinkHeader;

/// <summary>
/// The dns-prefetch and preconnect mode.
/// </summary>
public enum LinkHeaderDnsPrefetchPreconnectMode
{
	/// <summary>
	/// Outputs a dns-prefetch link header and a preconnect link header.
	/// </summary>
	Both,

	/// <summary>
	/// Outputs a dns-prefetch link header.
	/// </summary>
	DnsPrefetch,

	/// <summary>
	/// Outputs a preconnect link header.
	/// </summary>
	Preconnect
}