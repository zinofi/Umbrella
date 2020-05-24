using System.Collections.Generic;

namespace Umbrella.DynamicImage.Abstractions
{
	/// <summary>
	/// Contains utility methods for common operations performed by the Dynamic Image infrastructure.
	/// </summary>
	public interface IDynamicImageUtility
	{
		/// <summary>
		/// Parses the image format.
		/// </summary>
		/// <param name="format">The format. This is usually the file extension, optionally starting with a '.'</param>
		/// <returns>The <see cref="DynamicImageFormat"/> for the specified format.</returns>
		DynamicImageFormat ParseImageFormat(string format);

		/// <summary>
		/// Tries to the parse the relative URL into <see cref="DynamicImageOptions"/>.
		/// </summary>
		/// <param name="dynamicImagePathPrefix">The dynamic image path prefix, e.g. /dynamic-image</param>
		/// <param name="relativeUrl">The relative URL.</param>
		/// <param name="overrideFormat">
		/// The dynamic image format that the image will be set to regardless of the original format specified in the URL.
		/// This is useful for web situations where a browser supports webp and the url is requesting a jpg. This parameter allows the caller
		/// to override the image format and set it to webp.
		/// </param>
		/// <returns>A tuple containing the status of the parsing operation together with the parsed options.</returns>
		(DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) TryParseUrl(string dynamicImagePathPrefix, string relativeUrl, DynamicImageFormat? overrideFormat = null);
		bool ImageOptionsValid(DynamicImageOptions imageOptions, IEnumerable<DynamicImageMapping> validMappings);
		string GenerateVirtualPath(string dynamicImagePathPrefix, DynamicImageOptions options);
	}
}