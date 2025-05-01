namespace Umbrella.Utilities.Imaging.Abstractions;

/// <summary>
/// A helper used to support the generation of responsive images.
/// </summary>
public interface IResponsiveImageHelper
{
	/// <summary>
	/// Applies the pixel density to the image URL. This is used to transform the image URL into a pixel density specific URL.
	/// </summary>
	/// <param name="sanitizedImageUrl">The sanitized image URL.</param>
	/// <param name="pixelDensity">The pixel density.</param>
	/// <returns>The transformed image URL.</returns>
	string ApplyPixelDensity(string sanitizedImageUrl, int pixelDensity);

	/// <summary>
	/// Converts the <paramref name="itemsString"/> to a list of integers. Any items that cannot be parsed are discarded.
	/// </summary>
	/// <param name="itemsString">The items string.</param>
	/// <returns>The parsed items.</returns>
	IReadOnlyCollection<int> GetParsedIntegerItems(string itemsString);

	/// <summary>
	/// Gets the pixel densities below the maximum, i.e. between 1 and <paramref name="maxPixelDensity"/> inclusive.
	/// </summary>
	/// <param name="maxPixelDensity">The maximum pixel density value inclusive.</param>
	/// <returns>The pixel densities</returns>
	IReadOnlyCollection<int> GetPixelDensities(int maxPixelDensity);

	/// <summary>
	/// Takes the image URL and returns a single URL for the specified <paramref name="pixelDensity"/> e.g. /my-images/image@2x.png
	/// </summary>
	/// <param name="imageUrl">The image URL.</param>
	/// <param name="pixelDensity">The pixel density.</param>
	/// <param name="pixelDensityUrlTransformer">The optional pixel density URL transformer. If not provided, this will be applied using the <see cref="ApplyPixelDensity(string, int)"/> method.</param>
	/// <param name="pathResolver">The optional path resolver.</param>
	/// <returns>The image URL.</returns>
	string GetPixelDensityImageUrl(string imageUrl, int pixelDensity, Func<string, int, string>? pixelDensityUrlTransformer = null, Func<string, string>? pathResolver = null);

	/// <summary>
	/// Takes the image URL and returns a collection of URLs containing pixel densities between 1 and the specified maximum <paramref name="maxPixelDensity"/>, e.g.
	/// /my-images/image@2x.png
	/// </summary>
	/// <param name="imageUrl">The image URL.</param>
	/// <param name="maxPixelDensity">The maximum pixel density value inclusive.</param>
	/// <param name="pixelDensityUrlTransformer">The optional pixel density URL transformer. If not provided, this will be applied using the <see cref="ApplyPixelDensity(string, int)"/> method.</param>
	/// <param name="pathResolver">The path resolver.</param>
	/// <returns>The list of image URLs.</returns>
	IReadOnlyCollection<string> GetPixelDensityImageUrls(string imageUrl, int maxPixelDensity, Func<string, int, string>? pixelDensityUrlTransformer = null, Func<string, string>? pathResolver = null);

	/// <summary>
	/// Gets a string containing pixel densities which can be used in the srcset attribute of an HTML img tag.
	/// </summary>
	/// <param name="imageUrl">The image URL.</param>
	/// <param name="maxPixelDensity">The maximum pixel density value inclusive.</param>
	/// <param name="pixelDensityUrlTransformer">The optional pixel density URL transformer. If not provided, this will be applied using the <see cref="ApplyPixelDensity(string, int)"/> method.</param>
	/// <param name="pathResolver">The path resolver.</param>
	/// <returns>The srcset string.</returns>
	string GetPixelDensitySrcSetValue(string imageUrl, int maxPixelDensity, Func<string, int, string>? pixelDensityUrlTransformer = null, Func<string, string>? pathResolver = null);

	/// <summary>
	/// Gets a string containing size data which can be used in the srcset attribute of an HTML img tag.
	/// </summary>
	/// <param name="imageUrl">The image URL.</param>
	/// <param name="sizeWidths">The size widths.</param>
	/// <param name="maxPixelDensity">The maximum pixel density value inclusive.</param>
	/// <param name="widthRequest">The width request.</param>
	/// <param name="heightRequest">The height request.</param>
	/// <param name="pathResolver">The path resolver.</param>
	/// <returns>The srcset string.</returns>
	string GetSizeSrcSetValue(string imageUrl, string sizeWidths, int maxPixelDensity, int widthRequest, int heightRequest, Func<(string path, int imageWidth, int imageHeight), string> pathResolver);
}