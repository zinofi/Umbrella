using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;

namespace Umbrella.DynamicImage.Abstractions
{
    public abstract class DynamicImageResizerBase : IDynamicImageResizer
    {
        #region Protected Properties
        protected ILogger Log { get; }
        protected IDynamicImageCache Cache { get; }
        #endregion

        #region Constructors
        public DynamicImageResizerBase(ILogger logger,
            IDynamicImageCache dynamicImageCache)
        {
            Log = logger;
            Cache = dynamicImageCache;
        }
        #endregion

        #region IDynamicImageResizer Members
        public virtual async Task<DynamicImageItem> GenerateImageAsync(IUmbrellaFileProvider sourceFileProvider, DynamicImageOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
                Guard.ArgumentNotNull(sourceFileProvider, nameof(sourceFileProvider));

                var fileInfo = await sourceFileProvider.GetAsync(options.SourcePath, cancellationToken).ConfigureAwait(false);

                if (fileInfo == null)
                    return null;

                if (await fileInfo.ExistsAsync().ConfigureAwait(false))
                {
                    return await GenerateImageAsync(() => fileInfo.ReadAsByteArrayAsync(cancellationToken),
                        fileInfo.LastModified.Value,
                        options,
                        cancellationToken)
                        .ConfigureAwait(false);
                }

                return null;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { options }, returnValue: true))
            {
                throw new DynamicImageException("An error has occurred during image resizing.", exc, options);
            }
        }

        public virtual async Task<DynamicImageItem> GenerateImageAsync(Func<Task<byte[]>> sourceBytesProvider, DateTimeOffset sourceLastModified, DynamicImageOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
                Guard.ArgumentNotNull(sourceBytesProvider, nameof(sourceBytesProvider));

                if (Log.IsEnabled(LogLevel.Debug))
                    Log.WriteDebug(new { sourceLastModified, options }, "Started generating the image based on the recoreded state.");

                //Check if the image exists in the cache
                DynamicImageItem dynamicImage = await Cache.GetAsync(options, sourceLastModified, options.Format.ToFileExtensionString()).ConfigureAwait(false);

                if (Log.IsEnabled(LogLevel.Debug))
                    Log.WriteDebug(new { options, sourceLastModified, options.Format }, "Searched the image cache using the supplied state.");

                if (dynamicImage != null)
                {
                    if (Log.IsEnabled(LogLevel.Debug))
                        Log.WriteDebug(new { dynamicImage.ImageOptions, dynamicImage.LastModified }, "Image found in cache.");

                    return dynamicImage;
                }

                //Item cannot be found in the cache - build a new image
                byte[] originalBytes = await sourceBytesProvider().ConfigureAwait(false);

                //Need to get the newly resized image and assign it to the instance
                dynamicImage = new DynamicImageItem
                {
                    ImageOptions = options,
                    LastModified = DateTimeOffset.UtcNow
                };

                dynamicImage.Content = ResizeImage(originalBytes, options);

                //Now add to the cache
                await Cache.AddAsync(dynamicImage).ConfigureAwait(false);

                return dynamicImage;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { sourceLastModified, options }, returnValue: true))
            {
                throw new DynamicImageException("An error has occurred during image resizing.", exc, options);
            }
        }
        #endregion

        protected abstract byte[] ResizeImage(byte[] originalImage, DynamicImageOptions options);

        protected (int Width, int Height, int OffsetX, int OffsetY, int CropWidth, int CropHeight) GetDestinationDimensions(int originalWidth, int originalHeight, int targetWidth, int targetHeight, DynamicResizeMode mode)
        {
            int? requestedWidth = null;
            int? requestedHeight = null;

            int width = 0;
            int height = 0;
            int offsetX = 0;
            int offsetY = 0;
            int cropWidth = originalWidth;
            int cropHeight = originalHeight;

            switch (mode)
            {
                case DynamicResizeMode.UseWidth:
                    requestedWidth = targetWidth < originalWidth ? targetWidth : originalWidth;
                    break;
                case DynamicResizeMode.UseHeight:
                    requestedHeight = targetHeight < originalHeight ? targetHeight : originalHeight;
                    break;
                case DynamicResizeMode.Fill:
                    requestedWidth = targetWidth;
                    requestedHeight = targetHeight;
                    break;
                case DynamicResizeMode.Uniform:
                    // If both requested dimensions are greater than source image, we don't need to do any resizing.
                    if (targetWidth < originalWidth || targetHeight < originalHeight)
                    {
                        // Calculate requested width and height so as not to squash image.

                        // First, resize based on max width and check whether resized height will be more than max height.
                        var (tempWidth, tempHeight) = CalculateOutputDimensions(originalWidth, originalHeight, targetWidth, null);

                        if (tempHeight > targetHeight)
                        {
                            // If so, we need to resize based on max height instead.
                            requestedHeight = targetHeight;
                        }
                        else
                        {
                            // If not, we have our max dimension.
                            requestedWidth = targetWidth;
                        }
                    }
                    else
                    {
                        requestedWidth = originalWidth;
                        requestedHeight = originalHeight;
                    }
                    break;
                case DynamicResizeMode.UniformFill:
                    // Resize based on width first. If this means that height is less than target height, we resize based on height.
                    if (targetWidth < originalWidth || targetHeight < originalHeight)
                    {
                        // Calculate requested width and height so as not to squash image.

                        // First, resize based on width and check whether resized height will be more than max height.
                        var (tempWidth, tempHeight) = CalculateOutputDimensions(originalWidth, originalHeight, targetWidth, null);

                        if (tempHeight < targetHeight)
                        {
                            // If so, we need to resize based on max height instead.
                            requestedHeight = targetHeight;
                            //height = targetHeight;

                            (tempWidth, tempHeight) = CalculateOutputDimensions(originalWidth, originalHeight, null, targetHeight);

                            // Then crop width and calculate offset.
                            requestedWidth = targetWidth;
                            cropWidth = (int)((targetWidth / (float)tempWidth) * originalWidth);
                            offsetX = (originalWidth - cropWidth) / 2;
                        }
                        else
                        {
                            // If not, we have our max dimension.
                            requestedWidth = targetWidth;
                            //width = targetWidth;

                            // Then crop height and calculate offset.
                            requestedHeight = targetHeight;
                            cropHeight = (int)((targetHeight / (float)tempHeight) * originalHeight);
                            offsetY = (originalHeight - cropHeight) / 2;
                        }
                    }
                    else
                    {
                        requestedWidth = originalWidth;
                        requestedHeight = originalHeight;
                    }
                    break;
            }

            (width, height) = CalculateOutputDimensions(originalWidth, originalHeight, requestedWidth, requestedHeight);

            return (width, height, offsetX, offsetY, cropWidth, cropHeight);
        }

        private (int Width, int Height) CalculateOutputDimensions(int nInputWidth, int nInputHeight, int? nRequestedWidth, int? nRequestedHeight)
        {
            // both width and height are specified - squash image
            if (nRequestedWidth != null && nRequestedHeight != null)
            {
                return (nRequestedWidth.Value, nRequestedHeight.Value);
            }
            else if (nRequestedWidth != null) // calculate height to keep aspect ratio
            {
                double aspectRatio = (double)nInputWidth / nInputHeight;

                return (nRequestedWidth.Value, (int)(nRequestedWidth.Value / aspectRatio));
            }
            else if (nRequestedHeight != null) // calculate width to keep aspect ratio
            {
                double aspectRatio = (double)nInputHeight / nInputWidth;

                return ((int)(nRequestedHeight.Value / aspectRatio), nRequestedHeight.Value);
            }
            else
            {
                throw new Exception("Width or height, or both, must be specified");
            }
        }
    }
}