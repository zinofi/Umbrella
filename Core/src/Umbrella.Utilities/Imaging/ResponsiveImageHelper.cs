using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Imaging
{
	/// <summary>
	/// A helper used to support the generation of responsive images.
	/// </summary>
	public class ResponsiveImageHelper : IResponsiveImageHelper
	{
		private static readonly char[] _separatorCharacterArray = new[] { ',' };

		private readonly ILogger<ResponsiveImageHelper> _logger;

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
		public string GetPixelDensitySrcSetValue(string imageUrl, string pixelDensities, Func<string, string> pathResolver = null)
		{
			Guard.ArgumentNotNullOrWhiteSpace(imageUrl, nameof(imageUrl));
			Guard.ArgumentNotNullOrWhiteSpace(pixelDensities, nameof(pixelDensities));

			try
			{
				var lstPixelDensity = GetParsedIntegerItems(pixelDensities);

				if (lstPixelDensity.Count < 2)
					return string.Empty;

				int densityIndex = imageUrl.LastIndexOf('.');

				if (densityIndex == -1)
					return "Invalid image path";

				IEnumerable<string> srcsetImagePaths =
					from density in lstPixelDensity
					orderby density
					let densityX = $"{density}x"
					let highResImagePath = density > 1 ? imageUrl.Insert(densityIndex, $"@{densityX}") : imageUrl
					select pathResolver?.Invoke(highResImagePath) ?? highResImagePath + " " + densityX;

				return string.Join(", ", srcsetImagePaths);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { imageUrl, pixelDensities }, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem creating the srcset value.", exc);
			}
		}

		/// <inheritdoc />
		public string GetSizeSrcSetValue(string imageUrl, string sizeWidths, string pixelDensities, int widthRequest, int heightRequest, Func<(string path, int imageWidth, int imageHeight), string> pathResolver)
		{
			Guard.ArgumentNotNullOrWhiteSpace(imageUrl, nameof(imageUrl));
			Guard.ArgumentNotNullOrWhiteSpace(sizeWidths, nameof(sizeWidths));
			Guard.ArgumentNotNullOrWhiteSpace(pixelDensities, nameof(pixelDensities));
			Guard.ArgumentInRange(widthRequest, nameof(widthRequest), 1);
			Guard.ArgumentInRange(heightRequest, nameof(heightRequest), 1);
			Guard.ArgumentNotNull(pathResolver, nameof(pathResolver));

			try
			{
				var lstSize = new HashSet<string>();

				float aspectRatio = widthRequest / (float)heightRequest;

				foreach (int sizeWidth in GetParsedIntegerItems(sizeWidths))
				{
					foreach (int density in GetParsedIntegerItems(pixelDensities))
					{
						int imgWidth = sizeWidth * density;
						int imgHeight = (int)Math.Ceiling(imgWidth / aspectRatio);

						string resolvedUrl = pathResolver((imageUrl, imgWidth, imgHeight));

						lstSize.Add($"{resolvedUrl} {imgWidth}w");
					}
				}

				return string.Join(", ", lstSize.OrderBy(x => x));
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { imageUrl, sizeWidths, pixelDensities, widthRequest, heightRequest }, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem creating the srcset value.", exc);
			}
		}
	}
}