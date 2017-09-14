using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;

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
                var fileInfo = await sourceFileProvider.GetAsync(options.SourcePath, cancellationToken).ConfigureAwait(false);

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
    }
}