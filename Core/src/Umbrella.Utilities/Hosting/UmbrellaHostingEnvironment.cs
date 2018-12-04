using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Primitives;

namespace Umbrella.Utilities.Hosting
{
    public abstract class UmbrellaHostingEnvironment : IUmbrellaHostingEnvironment
    {
        #region Protected Properties
        protected ILogger Log { get; }
        protected IMemoryCache Cache { get; }
        protected ICacheKeyUtility CacheKeyUtility { get; }
        #endregion

        #region Constructors
        public UmbrellaHostingEnvironment(ILogger logger,
            IMemoryCache cache,
            ICacheKeyUtility cacheKeyUtility)
        {
            Log = logger;
            Cache = cache;
            CacheKeyUtility = cacheKeyUtility;
        }
        #endregion

        #region IUmbrellaHostingEnvironment Members
        public abstract string MapPath(string virtualPath, bool fromContentRoot = true);

        public virtual string GetFileContent(string virtualPath, bool fromContentRoot = true, bool cache = true, bool watch = true)
        {
            Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

            try
            {
                // TODO: Can we take advantage of the ArrayPool stuff to make this even better?
                string key = CacheKeyUtility.Create<UmbrellaHostingEnvironment>(new string[] { virtualPath, fromContentRoot.ToString(), cache.ToString() });
                
                string physicalPath = MapPath(virtualPath, fromContentRoot);

                string ReadContent()
                {
                    if (File.Exists(physicalPath))
                        return File.ReadAllText(physicalPath);

                    return null;
                }

                if (cache)
                {
                    return Cache.GetOrCreate(key, entry =>
                    {
                        entry.SetSlidingExpiration(TimeSpan.FromHours(1));

                        if(watch)
                            entry.AddExpirationToken(new PhysicalFileChangeToken(new FileInfo(physicalPath)));

                        return ReadContent();
                    });
                }
                else
                {
                    return ReadContent();
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, fromContentRoot, cache }))
            {
                throw;
            }
        } 
        #endregion
    }
}