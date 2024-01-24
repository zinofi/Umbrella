namespace Umbrella.WebUtilities.Middleware.Options.LinkHeader;

/// <summary>
/// A type that allows controlling the URL, rel and crossorigin attributes for preload link headers.
/// </summary>
/// <param name="Url">The URL.</param>
/// <param name="AsType">The preload "as" type.</param>
/// <param name="CrossOriginType">The cross origin type.</param>
public record LinkHeaderPreloadUrl(
	string Url,
	LinkHeaderPreloadAsType AsType,
	LinkHeaderCrossOriginType CrossOriginType = LinkHeaderCrossOriginType.Anonymous)
{
	/// <summary>
	/// Output this instance as a link header string.
	/// </summary>
	/// <returns>The link header string.</returns>
	/// <exception cref="ArgumentException">Thrown when the <see cref="CrossOriginType"/> has an unknown value.</exception>
	public string ToLinkHeaderString() => CrossOriginType switch
	{
		LinkHeaderCrossOriginType.None => $"<{Url}>; rel=preload; as={AsType.ToPreloadAsString()}",
		LinkHeaderCrossOriginType.Anonymous => $"<{Url}>; rel=preload; as={AsType.ToPreloadAsString()}; crossorigin",
		LinkHeaderCrossOriginType.UseCredentials => $"<{Url}>; rel=preload; as={AsType.ToPreloadAsString()}; crossorigin=use-credentials",
		_ => throw new ArgumentException($"The specified value: {CrossOriginType} is not supported.", nameof(CrossOriginType))
	};
}