namespace Umbrella.DynamicImage.Abstractions;

/// <summary>
/// The result of an attempt to parse a DynamicImage URL.
/// </summary>
public enum DynamicImageParseUrlResult
{
	/// <summary>
	/// Indicates that the URL should be skipped.
	/// </summary>
	Skip,

	/// <summary>
	/// Indicates the URL was successfully parsed.
	/// </summary>
	Success,

	/// <summary>
	/// Indicates that the URL could not be parsed and is therefore invalid.
	/// </summary>
	Invalid
}