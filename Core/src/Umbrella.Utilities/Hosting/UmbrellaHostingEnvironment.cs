using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Umbrella.Utilities.Hosting
{
    public abstract class UmbrellaHostingEnvironment : IUmbrellaHostingEnvironment
    {
        #region Private Static Members
        private static readonly string s_CacheKeyPrefix = typeof(UmbrellaHostingEnvironment).FullName;
        #endregion

        #region Protected Properties
        protected ILogger Log { get; }
        protected IMemoryCache Cache { get; }
        #endregion

        #region Constructors
        public UmbrellaHostingEnvironment(ILogger logger,
            IMemoryCache cache)
        {
            Log = logger;
            Cache = cache;
        }
        #endregion

        #region IUmbrellaHostingEnvironment Members
        public abstract string MapPath(string virtualPath, bool fromContentRoot = true);

        public virtual string GetFileContent(string virtualPath, bool fromContentRoot = true, bool cache = true)
        {
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

                string key = $"{s_CacheKeyPrefix}:{nameof(GetFileContent)}:{virtualPath}:{fromContentRoot}:{cache}".ToUpperInvariant();

                string ReadContent()
                {
                    string physicalPath = MapPath(virtualPath, fromContentRoot);

                    if (File.Exists(physicalPath))
                        return File.ReadAllText(physicalPath);

                    return null;
                }

                if (cache)
                {
                    return Cache.GetOrCreate(key, entry =>
                    {
                        entry.SetSlidingExpiration(TimeSpan.FromHours(1));

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