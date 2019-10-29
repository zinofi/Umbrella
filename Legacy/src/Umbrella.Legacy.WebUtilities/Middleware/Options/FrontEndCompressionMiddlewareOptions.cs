using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Owin;
using Umbrella.Utilities.Abstractions;

namespace Umbrella.Legacy.WebUtilities.Middleware.Options
{
	public class FrontEndCompressionMiddlewareOptions : CacheableUmbrellaOptions
	{
		// TODO: V3 Consider changing the arrays to HashSets for faster lookups
		public string[] FrontEndRootFolderAppRelativePaths { get; set; }
		public string[] TargetFileExtensions { get; set; } = new string[] { ".css", ".js" };
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
		public override CacheItemPriority CachePriority { get; set; } = CacheItemPriority.High;
	}
}