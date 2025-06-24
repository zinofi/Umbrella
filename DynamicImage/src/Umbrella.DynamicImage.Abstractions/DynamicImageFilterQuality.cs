namespace Umbrella.DynamicImage.Abstractions;

/// <summary>
/// The options for filter quality when resizing.
/// </summary>
public enum DynamicImageFilterQuality
{
	/// <summary>
	/// No filtering is applied. The image may appear pixelated when scaled.
	/// </summary>
	None = 0,

	/// <summary>
	/// Low quality filtering, bilinear filtering without mipmaps.
	/// </summary>
	Low = 1,

	/// <summary>
	/// Medium quality filtering, bilinear filtering with mipmaps.
	/// </summary>
	Medium = 2,

	/// <summary>
	/// High quality filtering, cubic resampling.
	/// </summary>
	High = 3
}