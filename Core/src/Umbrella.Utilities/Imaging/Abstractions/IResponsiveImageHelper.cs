using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Imaging
{
	/// <summary>
	/// A helper used to support the generation of responsive images.
	/// </summary>
	public interface IResponsiveImageHelper
	{
		/// <summary>
		/// Gets the <paramref name="itemsString"/> to a list of integers. Any items that cannot be parsed are discarded.
		/// </summary>
		/// <param name="itemsString">The items string.</param>
		/// <returns>The parsed items.</returns>
		IReadOnlyCollection<int> GetParsedIntegerItems(string itemsString);

		/// <summary>
		/// Gets a string containing pixel densities which can be used in the srcset attribute of an HTML img tag.
		/// </summary>
		/// <param name="imageUrl">The image URL.</param>
		/// <param name="pixelDensities">The pixel densities.</param>
		/// <param name="pathResolver">The path resolver.</param>
		/// <returns>The srcset string.</returns>
		string GetPixelDensitySrcSetValue(string imageUrl, string pixelDensities, Func<string, string> pathResolver = null);

		/// <summary>
		/// Gets a string containing size data which can be used in the srcset attribute of an HTML img tag.
		/// </summary>
		/// <param name="imageUrl">The image URL.</param>
		/// <param name="sizeWidths">The size widths.</param>
		/// <param name="pixelDensities">The pixel densities.</param>
		/// <param name="widthRequest">The width request.</param>
		/// <param name="heightRequest">The height request.</param>
		/// <param name="pathResolver">The path resolver.</param>
		/// <returns>The srcset string.</returns>
		string GetSizeSrcSetValue(string imageUrl, string sizeWidths, string pixelDensities, int widthRequest, int heightRequest, Func<(string path, int imageWidth, int imageHeight), string> pathResolver);
	}
}