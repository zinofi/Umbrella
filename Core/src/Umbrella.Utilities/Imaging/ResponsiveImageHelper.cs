// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Imaging;

/// <summary>
/// A helper used to support the generation of responsive images.
/// </summary>
public class ResponsiveImageHelper : IResponsiveImageHelper
{
	private static readonly char[] _separatorCharacterArray = new[] { ',' };

	private readonly ILogger<ResponsiveImageHelper> _logger;
	private readonly ConcurrentDictionary<int, int[]> _pixelDensityDictionary = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ResponsiveImageHelper"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public ResponsiveImageHelper(ILogger<ResponsiveImageHelper> logger)
	{
		_logger = logger;
	}

	/// <inheritdoc />
	public IReadOnlyCollection<int> GetParsedIntegerItems(string itemsString)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(itemsString))
				return Array.Empty<int>();

			string[] items = itemsString.Split(_separatorCharacterArray, StringSplitOptions.RemoveEmptyEntries);

			return new HashSet<int>(items.ParseIntegers());
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { itemsString }, returnValue: true))
		{
			throw new UmbrellaException("There has been a problem parsing the items.", exc);
		}
	}

	/// <inheritdoc />
	public IReadOnlyCollection<int> GetPixelDensities(int maxPixelDensity)
	{
		Guard.IsGreaterThanOrEqualTo(maxPixelDensity, 1, nameof(maxPixelDensity));

		try
		{
			return _pixelDensityDictionary.GetOrAdd(maxPixelDensity, key =>
			{
				int[] densities = new int[maxPixelDensity];

				for (int i = maxPixelDensity; i >= 1; i--)
				{
					densities[maxPixelDensity - i] = maxPixelDensity - i + 1;
				}

				return densities;
			});
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { maxPixelDensity }, returnValue: true))
		{
			throw new UmbrellaException("There has been a problem getting the pixel densities.", exc);
		}
	}

	/// <inheritdoc />
	public string GetPixelDensitySrcSetValue(string imageUrl, int maxPixelDensity, Func<string, string>? pathResolver = null)
	{
		Guard.IsNotNullOrWhiteSpace(imageUrl, nameof(imageUrl));
		Guard.IsGreaterThanOrEqualTo(maxPixelDensity, 1, nameof(maxPixelDensity));

		try
		{
			var lstPixelDensity = GetPixelDensities(maxPixelDensity);

			if (lstPixelDensity.Count < 2)
				return string.Empty;

			var (sanitizedImageUrl, qs) = SplitImageUrl(imageUrl);

			int densityIndex = sanitizedImageUrl.LastIndexOf('.');

			if (densityIndex is -1)
				return "Invalid image path";

			IEnumerable<string> srcsetImagePaths =
				from density in lstPixelDensity
				orderby density
				let densityX = $"{density}x"
				let highResImagePath = density > 1 ? sanitizedImageUrl.Insert(densityIndex, $"@{densityX}") : sanitizedImageUrl
				select (pathResolver?.Invoke(highResImagePath) ?? highResImagePath) + (!string.IsNullOrEmpty(qs) ? "?" + qs : null) + " " + densityX;

			return string.Join(", ", srcsetImagePaths);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { imageUrl, maxPixelDensity }, returnValue: true))
		{
			throw new UmbrellaException("There has been a problem creating the srcset value.", exc);
		}
	}

	/// <inheritdoc />
	public IReadOnlyCollection<string> GetPixelDensityImageUrls(string imageUrl, int maxPixelDensity, Func<string, string>? pathResolver = null)
	{
		Guard.IsNotNullOrWhiteSpace(imageUrl, nameof(imageUrl));
		Guard.IsGreaterThanOrEqualTo(maxPixelDensity, 1, nameof(maxPixelDensity));

		try
		{
			var lstPixelDensity = GetPixelDensities(maxPixelDensity);

			var (sanitizedImageUrl, qs) = SplitImageUrl(imageUrl);

			int densityIndex = sanitizedImageUrl.LastIndexOf('.');

			if (densityIndex is -1)
				throw new Exception("The image URL must contain a '.'");

			IEnumerable<string> paths =
				from density in lstPixelDensity
				orderby density
				let densityX = $"{density}x"
				let highResImagePath = density > 1 ? sanitizedImageUrl.Insert(densityIndex, $"@{densityX}") : sanitizedImageUrl
				select (pathResolver?.Invoke(highResImagePath) ?? highResImagePath) + (!string.IsNullOrEmpty(qs) ? "?" + qs : null);

			return paths.ToArray();
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { imageUrl, maxPixelDensity }, returnValue: true))
		{
			throw new UmbrellaException("There has been a problem creating the urls.", exc);
		}
	}

	/// <inheritdoc />
	public string GetPixelDensityImageUrl(string imageUrl, int pixelDensity, Func<string, string>? pathResolver = null)
	{
		Guard.IsNotNullOrWhiteSpace(imageUrl, nameof(imageUrl));
		Guard.IsGreaterThanOrEqualTo(pixelDensity, 1, nameof(pixelDensity));

		try
		{
			var (sanitizedImageUrl, qs) = SplitImageUrl(imageUrl);

			int densityIndex = sanitizedImageUrl.LastIndexOf('.');

			if (densityIndex is -1)
				throw new Exception("The image URL must contain a '.'");

			string highResImagePath = pixelDensity > 1 ? sanitizedImageUrl.Insert(densityIndex, $"@{pixelDensity}x") : sanitizedImageUrl;

			return (pathResolver?.Invoke(highResImagePath) ?? highResImagePath) + (!string.IsNullOrEmpty(qs) ? "?" + qs : null);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { imageUrl, pixelDensity }, returnValue: true))
		{
			throw new UmbrellaException("There has been a problem creating the url.", exc);
		}
	}

	private static (string sanitizedImageUrl, string? qs) SplitImageUrl(string imageUrl)
	{
		string sanitizedImageUrl = imageUrl;
		string? qs = null;

		if (imageUrl.Contains('?'))
		{
			string[] parts = imageUrl.Split('?');

			if (parts.Length != 2)
				throw new Exception("The path contains more than one '?'.");

			sanitizedImageUrl = parts[0];
			qs = parts[1];
		}

		return (sanitizedImageUrl, qs);
	}

	/// <inheritdoc />
	public string GetSizeSrcSetValue(string imageUrl, string sizeWidths, int maxPixelDensity, int widthRequest, int heightRequest, Func<(string path, int imageWidth, int imageHeight), string> pathResolver)
	{
		Guard.IsNotNullOrWhiteSpace(imageUrl, nameof(imageUrl));
		Guard.IsNotNullOrWhiteSpace(sizeWidths, nameof(sizeWidths));
		Guard.IsGreaterThanOrEqualTo(maxPixelDensity, 1, nameof(maxPixelDensity));
		Guard.IsGreaterThanOrEqualTo(widthRequest, 1, nameof(widthRequest));
		Guard.IsGreaterThanOrEqualTo(heightRequest, 1, nameof(heightRequest));
		Guard.IsNotNull(pathResolver, nameof(pathResolver));

		try
		{
			var lstSize = new HashSet<string>();

			float aspectRatio = widthRequest / (float)heightRequest;

			foreach (int sizeWidth in GetParsedIntegerItems(sizeWidths))
			{
				foreach (int density in GetPixelDensities(maxPixelDensity))
				{
					int imgWidth = sizeWidth * density;
					int imgHeight = (int)Math.Ceiling(imgWidth / aspectRatio);

					string resolvedUrl = pathResolver((imageUrl, imgWidth, imgHeight));

					_ = lstSize.Add($"{resolvedUrl} {imgWidth}w");
				}
			}

			return string.Join(", ", lstSize.OrderBy(x => x));
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { imageUrl, sizeWidths, maxPixelDensity, widthRequest, heightRequest }, returnValue: true))
		{
			throw new UmbrellaException("There has been a problem creating the srcset value.", exc);
		}
	}
}