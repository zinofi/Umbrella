namespace Umbrella.DynamicImage.Abstractions
{
	/// <summary>
	/// The formats supported by the Dynamic Image infrastructure.
	/// </summary>
	public enum DynamicImageFormat
	{
		/// <summary>
		/// A bitmap file.
		/// </summary>
		Bmp = 0,

		/// <summary>
		/// A gif file.
		/// </summary>
		Gif = 1,

		/// <summary>
		/// A jpeg / jpg file.
		/// </summary>
		Jpeg = 2,

		/// <summary>
		/// A png file.
		/// </summary>
		Png = 3,

		/// <summary>
		/// A webp file.
		/// </summary>
		WebP = 4
	}

	/// <summary>
	/// Contains extension methods that operate on values of the <see cref="DynamicImageFormat"/> enumeration.
	/// </summary>
	public static class DynamicImageFormatExtensions
	{
		/// <summary>
		/// Converts a <see cref="DynamicImageFormat"/> value to its corresponding file extension (without a leading '.').
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The file extension (without a leading '.').</returns>
		public static string ToFileExtensionString(this DynamicImageFormat value) => value switch
		{
			DynamicImageFormat.Jpeg => "jpg",
			_ => value.ToString().ToLowerInvariant(),
		};
	}
}