using Microsoft.Extensions.FileProviders;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.Middleware.Options
{
	// TODO: V3 - Consider moving to the common WebUtilities project for reuse with ASP.NET Core
	// Also, prefix the MultiCache properties so they can be distinguised from other caching flags.
    public class FrontEndCompressionMiddlewareOptions
    {
		// TODO: V3 Consider changing the arrays to HashSets for faster lookups
        public string[] FrontEndRootFolderAppRelativePaths { get; set; }
        public string[] TargetFileExtensions { get; set; } = new string[] { ".css", ".js" };
        public bool CacheEnabled { get; set; } = true;
        public TimeSpan CacheTimeout { get; set; } = TimeSpan.FromHours(1);
        public bool CacheSlidingExpiration { get; set; } = true;
        public bool WatchFiles { get; set; }
        public string AcceptEncodingHeaderKey { get; set; } = "Accept-Encoding";
        public Action<IOwinContext, HashSet<string>> AcceptEncodingModifier { get; set; }
		public int? MaxAgeSeconds { get; set; } = 31557600;
		public FrontEndCompressionMiddlewareHttpCacheability HttpCacheability { get; set; } = FrontEndCompressionMiddlewareHttpCacheability.Private;
		public bool MustRevalidate { get; set; } = true;
		public bool ResponseCacheEnabled { get; set; } = true;
		public bool CompressionEnabled { get; set; } = true;
		public int BufferSizeBytes { get; set; } = 81920;
		public Func<IOwinContext, IFileInfo, bool> ResponseCacheDeterminer { get; set; }
	}
}