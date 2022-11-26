using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Imaging;

/// <summary>
/// A helper used to support the generation of responsive images.
/// </summary>
public interface IResponsiveImageHelper
{
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
	/// <param name="pathResolver">The path resolver.</param>
	/// <returns></returns>
	string GetPixelDensityImageUrl(string imageUrl, int pixelDensity, Func<string, string>? pathResolver = null);

	/// <summary>
	/// Takes the image URL and returns a collection of URLs containing pixel densities between 1 and the specified maximum <paramref name="maxPixelDensity"/>, e.g.
	/// /my-images/image@2x.png
	/// </summary>
	/// <param name="imageUrl">The image URL.</param>
	/// <param name="maxPixelDensity">The maximum pixel density value inclusive.</param>
	/// <param name="pathResolver">The path resolver.</param>
	/// <returns>The list of image URLs.</returns>
	IReadOnlyCollection<string> GetPixelDensityImageUrls(string imageUrl, int maxPixelDensity, Func<string, string>? pathResolver = null);

	/// <summary>
	/// Gets a string containing pixel densities which can be used in the srcset attribute of an HTML img tag.
	/// </summary>
	/// <param name="imageUrl">The image URL.</param>
	/// <param name="maxPixelDensity">The maximum pixel density value inclusive.</param>
	/// <param name="pathResolver">The path resolver.</param>
	/// <returns>The srcset string.</returns>
	string GetPixelDensitySrcSetValue(string imageUrl, int maxPixelDensity, Func<string, string>? pathResolver = null);

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