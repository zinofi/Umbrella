using System;
using System.Drawing;
using System.IO;
using FreeImageAPI;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.DynamicImage.Abstractions.Caching;
using Umbrella.Utilities;

namespace Umbrella.DynamicImage.FreeImage
{
	// TODO: Handle issues with transparency information being lost on resize
	public class DynamicImageResizer : DynamicImageResizerBase
	{
		#region Constructors
		public DynamicImageResizer(
			ILogger<DynamicImageResizer> logger,
			IDynamicImageCache dynamicImageCache)
			: base(logger, dynamicImageCache)
		{
		}
		#endregion

		#region Overridden Methods
		public override bool IsImage(byte[] bytes)
		{
			try
			{
				Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));

				using var inputStream = new MemoryStream(bytes);
				using var image = FreeImageBitmap.FromStream(inputStream);

				return image != null;
			}
			catch
			{
				return false;
			}
		}

		public override byte[] ResizeImage(byte[] originalImage, int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat format)
		{
			try
			{
				Guard.ArgumentNotNullOrEmpty(originalImage, nameof(originalImage));

				using var inputStream = new MemoryStream(originalImage);
				using var image = FreeImageBitmap.FromStream(inputStream);

				FreeImageBitmap imageToSave = image;

				var result = GetDestinationDimensions(image.Width, image.Height, width, height, resizeMode);

				try
				{
					if (result.offsetX > 0 || result.offsetY > 0)
						imageToSave = image.Copy(new Rectangle(result.offsetX, result.offsetY, result.cropWidth, result.cropHeight));

					imageToSave.Rescale(result.width, result.height, FREE_IMAGE_FILTER.FILTER_LANCZOS3);

					using var outputStream = new MemoryStream();

					imageToSave.Save(outputStream, GetImageFormat(format), GetSaveFlags(format));

					return outputStream.ToArray();
				}
				finally
				{
					if (!ReferenceEquals(image, imageToSave))
						imageToSave.Dispose();
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { width, height, resizeMode, format }, returnValue: true))
			{
				throw new DynamicImageException("An error has occurred during image resizing.", exc, width, height, resizeMode, format);
			}
		}
		#endregion

		#region Private Methods
		private FREE_IMAGE_FORMAT GetImageFormat(DynamicImageFormat format) => format switch
		{
			DynamicImageFormat.Bmp => FREE_IMAGE_FORMAT.FIF_BMP,
			DynamicImageFormat.Gif => FREE_IMAGE_FORMAT.FIF_GIF,
			DynamicImageFormat.Jpeg => FREE_IMAGE_FORMAT.FIF_JPEG,
			DynamicImageFormat.Png => FREE_IMAGE_FORMAT.FIF_PNG,
			DynamicImageFormat.WebP => throw new NotSupportedException("WebP is not supported."),
			_ => default,
		};

		private FREE_IMAGE_SAVE_FLAGS GetSaveFlags(DynamicImageFormat format) => format switch
		{
			DynamicImageFormat.Jpeg => FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD | FREE_IMAGE_SAVE_FLAGS.JPEG_BASELINE,
			DynamicImageFormat.Png => FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION,
			DynamicImageFormat.WebP => throw new NotSupportedException("WebP is not supported."),
			_ => default
		};
		#endregion
	}
}