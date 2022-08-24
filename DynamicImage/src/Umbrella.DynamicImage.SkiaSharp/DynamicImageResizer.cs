using System;
using System.IO;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Abstractions.Caching;
using Umbrella.Utilities;

namespace Umbrella.DynamicImage.SkiaSharp
{
	/// <summary>
	/// An implementation of the <see cref="DynamicImageResizerBase"/> which uses SkiaSharp.
	/// </summary>
	/// <seealso cref="Umbrella.DynamicImage.Abstractions.DynamicImageResizerBase" />
	public class DynamicImageResizer : DynamicImageResizerBase
	{
		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicImageResizer"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dynamicImageCache">The dynamic image cache.</param>
		public DynamicImageResizer(
			ILogger<DynamicImageResizer> logger,
			IDynamicImageCache dynamicImageCache)
			: base(logger, dynamicImageCache)
		{
		}
		#endregion

		#region Overridden Methods
		/// <inheritdoc />
		public override bool IsImage(byte[] bytes)
		{
			try
			{
				Guard.IsNotNullOrEmpty(bytes, nameof(bytes));

				using var image = LoadBitmap(bytes);

				return image != null;
			}
			catch
			{
				return false;
			}
		}

		/// <inheritdoc />
		public override byte[] ResizeImage(byte[] originalImage, int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat format)
		{
			Guard.IsNotNullOrEmpty(originalImage, nameof(originalImage));

			try
			{
				using var image = LoadBitmap(originalImage);

				SKBitmap imageToResize = image;

				var result = GetDestinationDimensions(image.Width, image.Height, width, height, resizeMode);

				try
				{
					// TODO: Look at how we can alter the resizing code to allow the cropped area position to be varied.
					// Could have a crop hotspot, i.e. an X and Y coordinate that we use to specify the center of the cropped area and then
					// calculate the edges accordingly.
					if (result.offsetX > 0 || result.offsetY > 0)
					{
						var cropRect = SKRectI.Create(result.offsetX, result.offsetY, result.cropWidth, result.cropHeight);
						
						imageToResize = new SKBitmap(cropRect.Width, cropRect.Height);
						image.ExtractSubset(imageToResize, cropRect);
					}

					using var resizedImage = imageToResize.Resize(new SKImageInfo(result.width, result.height), SKFilterQuality.High);
					using var outputImage = SKImage.FromBitmap(resizedImage);

					// TODO: Allow the quality to be passed in as a parameter.
					return outputImage.Encode(GetImageFormat(format), 75).ToArray();
				}
				finally
				{
					if (!ReferenceEquals(image, imageToResize))
						imageToResize.Dispose();
				}
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { width, height, resizeMode, format }, returnValue: true))
			{
				throw new DynamicImageException("An error has occurred during image resizing.", exc, width, height, resizeMode, format);
			}
		}
		#endregion

		#region Private Methods
		private SKBitmap LoadBitmap(byte[] bytes)
		{
			using var ms = new MemoryStream(bytes);

			// NB: This breaks using 20.8.3. Using the replacement SkData.Create fixes things.
			// See: https://github.com/mono/SkiaSharp/issues/1551
			//using var s = new SKManagedStream(ms);
			//using var codec = SKCodec.Create(ms);

			using var skData = SKData.Create(ms);
			using var codec = SKCodec.Create(skData);

			var info = codec.Info;
			var bitmap = new SKBitmap(new SKImageInfo(info.Width, info.Height, info.ColorType, info.AlphaType, info.ColorSpace));

			var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out IntPtr length));

			return result switch
			{
				SKCodecResult.Success => bitmap,
				SKCodecResult.IncompleteInput => bitmap,
				_ => throw new ArgumentException("Unable to load bitmap from provided data")
			};
		}

		private SKEncodedImageFormat GetImageFormat(DynamicImageFormat format) => format switch
		{
			DynamicImageFormat.Bmp => SKEncodedImageFormat.Bmp,
			DynamicImageFormat.Gif => SKEncodedImageFormat.Gif,
			DynamicImageFormat.Jpeg => SKEncodedImageFormat.Jpeg,
			DynamicImageFormat.Png => SKEncodedImageFormat.Png,
			DynamicImageFormat.WebP => SKEncodedImageFormat.Webp,
			_ => default,
		};
		#endregion
	}
}