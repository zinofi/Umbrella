namespace Umbrella.WebUtilities.Middleware.Options.LinkHeader;

/// <summary>
/// A type that allows controlling the URL and crossorigin attribute for dns-prefetch and preconnect link headers.
/// </summary>
/// <param name="Url">The URL.</param>
/// <param name="Mode">The dns-prefetch and preconnect mode.</param>
/// <param name="CrossOriginType">The cross origin type.</param>
public record LinkHeaderDnsPrefetchPreconnectUrl(
	string Url,
	LinkHeaderDnsPrefetchPreconnectMode Mode = LinkHeaderDnsPrefetchPreconnectMode.Both,
	LinkHeaderCrossOriginType CrossOriginType = LinkHeaderCrossOriginType.Anonymous)
{
	/// <summary>
	/// Gets the link header strings for this instance.
	/// </summary>
	/// <returns>The link header strings.</returns>
	/// <exception cref="ArgumentException">Thrown when the <see cref="Mode"/> or <see cref="CrossOriginType"/> has an unknown value.</exception>
	public IReadOnlyCollection<string> ToLinkHeaderStrings() => Mode switch
	{
		LinkHeaderDnsPrefetchPreconnectMode.Both => [ToDnsPrefetchLinkHeaderString(), ToPreconnectLinkHeaderString()],
		LinkHeaderDnsPrefetchPreconnectMode.DnsPrefetch => [ToDnsPrefetchLinkHeaderString()],
		LinkHeaderDnsPrefetchPreconnectMode.Preconnect => [ToPreconnectLinkHeaderString()],
		_ => throw new ArgumentException($"The specified value: {Mode} is not supported.", nameof(Mode))
	};

	private string ToDnsPrefetchLinkHeaderString() => CrossOriginType switch
	{
		LinkHeaderCrossOriginType.None => $"<{Url}>; rel=dns-prefetch",
		LinkHeaderCrossOriginType.Anonymous => $"<{Url}>; rel=dns-prefetch; crossorigin",
		LinkHeaderCrossOriginType.UseCredentials => $"<{Url}>; rel=dns-prefetch; crossorigin=use-credentials",
		_ => throw new ArgumentException($"The specified value: {CrossOriginType} is not supported.", nameof(CrossOriginType))
	};

	private string ToPreconnectLinkHeaderString() => CrossOriginType switch
	{
		LinkHeaderCrossOriginType.None => $"<{Url}>; rel=preconnect",
		LinkHeaderCrossOriginType.Anonymous => $"<{Url}>; rel=preconnect; crossorigin",
		LinkHeaderCrossOriginType.UseCredentials => $"<{Url}>; rel=preconnect; crossorigin=use-credentials",
		_ => throw new ArgumentException($"The specified value: {CrossOriginType} is not supported.", nameof(CrossOriginType))
	};
}