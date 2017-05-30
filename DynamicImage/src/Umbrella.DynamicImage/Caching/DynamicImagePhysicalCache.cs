using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.Disk;
using Umbrella.Utilities.Mime;

namespace Umbrella.DynamicImage.Caching
{
    public abstract class DynamicImagePhysicalCache : DynamicImageCache, IDynamicImageCache
    {
        #region Public Properties
        public virtual string CachePathFormat => @"\{0}.{1}";
        #endregion

        #region Protected Properties
        protected IUmbrellaFileProvider FileProvider { get; }
        #endregion

        #region Constructors
        public DynamicImagePhysicalCache(ILogger logger,
            IMemoryCache cache,
            DynamicImageCacheOptions cacheOptions,
            IUmbrellaFileProvider fileProvider)
            : base(logger, cache, cacheOptions)
        {
            FileProvider = fileProvider;
        }
        #endregion

        #region IDynamicImageCache Members
        public virtual async Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                string key = GenerateCacheKey(dynamicImage.ImageOptions);

                //Save to disk - overwrite any existing entry
                string subPath = GetSubPath(key, dynamicImage.ImageOptions.Format.ToFileExtensionString());

                //Read the image content to save to the underlying file store
                byte[] bytes = await dynamicImage.GetContentAsync(cancellationToken).ConfigureAwait(false);

                await FileProvider.SaveAsync(subPath, bytes, false, cancellationToken).ConfigureAwait(false);
            }
            catch(Exception exc) when(Log.WriteError(exc, new { dynamicImage.ImageOptions }, returnValue: true))
            {
                throw new DynamicImageException($"There was a problem adding the {nameof(DynamicImageItem)} to the cache.", exc, dynamicImage.ImageOptions);
            }
        }

        public virtual async Task<DynamicImageItem> GetAsync(DynamicImageOptions options, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                string cacheKey = GenerateCacheKey(options);

                string subPath = GetSubPath(cacheKey, fileExtension);

                //Find the cached file
                IUmbrellaFileInfo fileInfo = await FileProvider.GetAsync(subPath, cancellationToken).ConfigureAwait(false);

                //No cached image available
                if (fileInfo == null)
                    return null;

                //If the file does not exist or has been modified since the item was generated,
                //evict it from the cache, i.e. delete the cached image from disk
                if (sourceLastModified > fileInfo.LastModified)
                {
                    await fileInfo.DeleteAsync(cancellationToken).ConfigureAwait(false);

                    return null;
                }

                //We need to return the cached image
                DynamicImageItem item = new DynamicImageItem
                {
                    LastModified = fileInfo.LastModified.Value,
                    Length = fileInfo.Length,
                    ImageOptions = options
                };

                //Set the content resolver to allow the file to be read from disk if / when needed
                item.SetContentResolver(async token =>
                {
                    token.ThrowIfCancellationRequested();

                    IUmbrellaFileInfo cachedFile = await FileProvider.GetAsync(subPath, token).ConfigureAwait(false);
                    
                    //Check the file still exists
                    if (cachedFile == null)
                        return null;

                    return await cachedFile.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                });

                return item;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { options, sourceLastModified, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was problem retrieving the image from the cache.", exc);
            }
        }

        public virtual async Task RemoveAsync(DynamicImageOptions options, string fileExtension, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                string cacheKey = GenerateCacheKey(options);

                string subPath = GetSubPath(cacheKey, fileExtension);
                await FileProvider.DeleteAsync(subPath).ConfigureAwait(false);
            }
            catch (Exception exc) when(Log.WriteError(exc, new { options, fileExtension }, returnValue: true))
            {
                throw new DynamicImageException("There was problem removing the image from the cache.", exc);
            }
        }
        #endregion

        #region Overridden Methods
        protected override string GetSubPath(string cacheKey, string fileExtension)
            => $@"\{cacheKey}.{fileExtension}";
        #endregion
    }
}