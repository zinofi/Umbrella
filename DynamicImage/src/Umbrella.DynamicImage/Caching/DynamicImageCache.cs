using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Umbrella.DynamicImage.Caching
{
    public abstract class DynamicImageCache
    {
        #region Private Static Members
        private static readonly SHA256 s_Hasher = SHA256.Create();
        #endregion

        #region Protected Properties
        protected ILogger Log { get; }
        protected IMemoryCache Cache { get; }
        protected DynamicImageCacheOptions CacheOptions { get; }
        #endregion

        #region Constructors
        public DynamicImageCache(ILogger logger,
            IMemoryCache cache,
            DynamicImageCacheOptions cacheOptions)
        {
            Log = logger;
            Cache = cache;
            CacheOptions = cacheOptions;

            if (cacheOptions.CacheKeyCacheOptions == null)
                throw new DynamicImageException($"{nameof(cacheOptions.CacheKeyCacheOptions)} cannot be null.");
        }
        #endregion

        #region Protected Methods
        protected virtual string GenerateCacheKey(DynamicImageOptions options)
        {
            try
            {
                return Cache.GetOrCreate(options, entry =>
                    {
                        entry.SetOptions(CacheOptions.CacheKeyCacheOptions);

                        string rawKey = string.Format("{0}-W-{1}-H-{2}-M-{3}-F-{4}-P", options.Width, options.Height, options.ResizeMode, options.Format, options.SourcePath);

                        byte[] bytes = s_Hasher.ComputeHash(Encoding.UTF8.GetBytes(rawKey));

                        StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);

                        foreach (byte num in bytes)
                            stringBuilder.Append(num.ToString("x").PadLeft(2, '0'));

                        return stringBuilder.ToString();
                    });
            }
            catch (Exception exc) when (Log.WriteError(exc, new { options }, returnValue: true))
            {
                throw new DynamicImageException("There was a problem generating the cache key.", exc, options);
            }
        }
        
        protected virtual string GetSubPath(string cackeKey, string fileExtension)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}