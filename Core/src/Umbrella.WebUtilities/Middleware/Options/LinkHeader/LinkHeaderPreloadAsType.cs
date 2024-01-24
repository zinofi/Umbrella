namespace Umbrella.WebUtilities.Middleware.Options.LinkHeader;

/// <summary>
/// The preload "as" type.
/// </summary>
public enum LinkHeaderPreloadAsType
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

/// <summary>
/// Extension methods for the <see cref="LinkHeaderPreloadAsType"/> enum type.
/// </summary>
public static class LinkHeaderPreloadAsTypeExtensions
{
	/// <summary>
	/// Converts to the enum value to its corresponding as attribute value.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The as attribute value.</returns>
	/// <exception cref="ArgumentException">Thrown when the <paramref name="value"/> has an unknown value.</exception>
	public static string ToPreloadAsString(this LinkHeaderPreloadAsType value) => value switch
	{
		LinkHeaderPreloadAsType.Audio => "audio",
		LinkHeaderPreloadAsType.Document => "document",
		LinkHeaderPreloadAsType.Embed => "embed",
		LinkHeaderPreloadAsType.Fetch => "fetch",
		LinkHeaderPreloadAsType.Font => "font",
		LinkHeaderPreloadAsType.Image => "image",
		LinkHeaderPreloadAsType.Object => "object",
		LinkHeaderPreloadAsType.Script => "script",
		LinkHeaderPreloadAsType.Style => "style",
		LinkHeaderPreloadAsType.Track => "track",
		LinkHeaderPreloadAsType.Worker => "worker",
		LinkHeaderPreloadAsType.Video => "video",
		_ => throw new ArgumentException($"The specified value: {value} is not supported.", nameof(value))
	};
}