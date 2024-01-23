using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.WebUtilities.Middleware.Options;

/// <summary>
/// Options for use with associated middleware in ASP.NET and ASP.NET Core projects.
/// </summary>
public class LinkHeaderMiddlewareOptions : ISanitizableUmbrellaOptions
{
    private static readonly IReadOnlyDictionary<LinkHeaderMiddlewarePreloadAsType, string> _linkHeaderMiddlewarePreloadAsTypeMappings = new Dictionary<LinkHeaderMiddlewarePreloadAsType, string>
    {
        [LinkHeaderMiddlewarePreloadAsType.Audio] = "audio",
        [LinkHeaderMiddlewarePreloadAsType.Document] = "document",
        [LinkHeaderMiddlewarePreloadAsType.Embed] = "embed",
        [LinkHeaderMiddlewarePreloadAsType.Fetch] = "fetch",
        [LinkHeaderMiddlewarePreloadAsType.Font] = "font",
        [LinkHeaderMiddlewarePreloadAsType.Image] = "image",
        [LinkHeaderMiddlewarePreloadAsType.Object] = "object",
        [LinkHeaderMiddlewarePreloadAsType.Script] = "script",
        [LinkHeaderMiddlewarePreloadAsType.Style] = "style",
        [LinkHeaderMiddlewarePreloadAsType.Track] = "track",
        [LinkHeaderMiddlewarePreloadAsType.Worker] = "worker",
        [LinkHeaderMiddlewarePreloadAsType.Video] = "video"
    };

	private static readonly IReadOnlyDictionary<LinkHeaderCrossOriginType, string> _linkHeaderCrossOriginTypeMappings = new Dictionary<LinkHeaderCrossOriginType, string>
	{
		[LinkHeaderCrossOriginType.None] = "none",
		[LinkHeaderCrossOriginType.Anonymous] = "anonymous",
		[LinkHeaderCrossOriginType.UseCredentials] = "use-credentials"
	};

    /// <summary>
    /// The URLs to be output as <c>Link</c> headers with rel=dns-prefetch values.
    /// </summary>
    public HashSet<string> DnsPrefetchUrls { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The URLs to be output as <c>Link</c> headers with rel=preconnect values.
    /// </summary>
    public HashSet<string> PreconnectUrls { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Specifies whether the ingestion endpoints for Application Insights services
    /// should be included.
    /// </summary>
    /// <remarks>
    /// Defaults to <see langword="true" />
    /// </remarks>
    public bool AddApplicationInsightsEndpoints { get; set; } = true;

    /// <inheritdoc/>
    public void Sanitize()
    {
        HashSet<string> lstSanitizedDnsPrefetchUrl = new(DnsPrefetchUrls.Select(x => x.TrimToLowerInvariant()));
        HashSet<string> lstSanitizedPreconnectUrl = new(PreconnectUrls.Select(x => x.TrimToLowerInvariant()));

        if (AddApplicationInsightsEndpoints)
        {
            _ = lstSanitizedPreconnectUrl.Add("https://js.monitor.azure.com");
            _ = lstSanitizedPreconnectUrl.Add("https://dc.services.visualstudio.com");
        }

        // TODO: Need to add a new record that allows controlling the URL, rel and crossorigin attributes for dns-prefetch and preconnect.
        // TODO: Need to add a new record that allows controlling the URL, rel, crossorigin and as attributes for preload.

        PreconnectUrls.Clear();
        PreconnectUrls.AddRange(lstSanitizedPreconnectUrl);
    }

	// TODO: Make extension methods.

	/// <summary>
	/// Gets the value of the <c>as</c> attribute for the specified <see cref="LinkHeaderMiddlewarePreloadAsType"/>.
	/// </summary>
	/// <param name="preloadAsType">The preload as type.</param>
	/// <returns>The value of the <c>as</c> attribute for the specified <see cref="LinkHeaderMiddlewarePreloadAsType"/>.</returns>
	public static string GetPreloadAsAttributeValue(LinkHeaderMiddlewarePreloadAsType preloadAsType) => _linkHeaderMiddlewarePreloadAsTypeMappings[preloadAsType];

	/// <summary>
	/// Gets the value of the <c>crossorigin</c> attribute for the specified <see cref="LinkHeaderCrossOriginType"/>.
	/// </summary>
	/// <param name="crossOriginType">The cross origin type.</param>
	/// <returns>The value of the <c>crossorigin</c> attribute for the specified <see cref="LinkHeaderCrossOriginType"/>.</returns>
	public static string GetCrossOriginAttributeValue(LinkHeaderCrossOriginType crossOriginType) => _linkHeaderCrossOriginTypeMappings[crossOriginType];
}

/// <summary>
/// A record that allows controlling the URL, rel and crossorigin attributes for preload link headers.
/// </summary>
/// <param name="Url">The URL.</param>
/// <param name="AsType">The preload "as" type.</param>
/// <param name="CrossOriginType">The cross origin type.</param>
public record LinkHeaderMiddlewarePreloadRecord(string Url, LinkHeaderMiddlewarePreloadAsType AsType, LinkHeaderCrossOriginType CrossOriginType = LinkHeaderCrossOriginType.Anonymous);

/// <summary>
/// A record that allows controlling the URL and crossorigin attribute for dns-prefetch and preconnect link headers.
/// </summary>
/// <param name="Url">The URL.</param>
/// <param name="CrossOriginType">The cross origin type.</param>
public record LinkHeaderMiddlewarePreconnectRecord(string Url, LinkHeaderCrossOriginType CrossOriginType = LinkHeaderCrossOriginType.Anonymous);

/// <summary>
/// The cross origin type.
/// </summary>
public enum LinkHeaderCrossOriginType
{
	/// <summary>
	/// No cross origin type.
	/// </summary>
	None,

	/// <summary>
	/// Anonymous cross origin type.
	/// </summary>
	Anonymous,

	/// <summary>
	/// Use credentials cross origin type.
	/// </summary>
	UseCredentials
}

/// <summary>
/// The preload "as" type.
/// </summary>
public enum LinkHeaderMiddlewarePreloadAsType
{
    /// <summary>
    /// Audio file.
    /// </summary>
    Audio,

    /// <summary>
    /// An HTML document intended to be embedded by a <![CDATA[<frame>]]> or <![CDATA[<iframe>]]> .
    /// </summary>
    Document,

    /// <summary>
    /// A resource to be embedded inside an <![CDATA[<embed>]]> element.
    /// </summary>
    Embed,

    /// <summary>
    /// Resource to be accessed by a fetch or XHR request, such as an ArrayBuffer or JSON file.
    /// </summary>
    Fetch,

    /// <summary>
    /// Font file.
    /// </summary>
    Font,

    /// <summary>
    /// Image file.
    /// </summary>
    Image,

    /// <summary>
    /// A resource to be embedded inside an <![CDATA[<object>]]> element.
    /// </summary>
    Object,

    /// <summary>
    /// JavaScript file.
    /// </summary>
    Script,

    /// <summary>
    /// CSS stylesheet.
    /// </summary>
    Style,

    /// <summary>
    /// WebVTT file.
    /// </summary>
    Track,

    /// <summary>
    /// A JavaScript web worker or shared worker.
    /// </summary>
    Worker,

    /// <summary>
    /// Video file.
    /// </summary>
    Video
}