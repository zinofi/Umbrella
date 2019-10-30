using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Umbrella.Utilities;
using Umbrella.Utilities.Options;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.WebUtilities.Middleware.Options
{
	public class FrontEndCompressionMiddlewareOptions : CacheableUmbrellaOptions, ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		public string[] FrontEndRootFolderAppRelativePaths { get; set; }
		public string[] TargetFileExtensions { get; set; } = new string[] { ".css", ".js" };
		public bool WatchFiles { get; set; }
		public string AcceptEncodingHeaderKey { get; set; } = "Accept-Encoding";
		public Action<IReadOnlyDictionary<string, IEnumerable<string>>, HashSet<string>> AcceptEncodingModifier { get; set; }
		public int? MaxAgeSeconds { get; set; } = 31557600;
		public FrontEndCompressionMiddlewareHttpCacheability HttpCacheability { get; set; } = FrontEndCompressionMiddlewareHttpCacheability.Private;
		public bool MustRevalidate { get; set; } = true;
		public bool ResponseCacheEnabled { get; set; } = true;
		public bool CompressionEnabled { get; set; } = true;
		public int BufferSizeBytes { get; set; } = 81920;
		public Func<IFileInfo, bool> ResponseCacheDeterminer { get; set; }
		public override CacheItemPriority CachePriority { get; set; } = CacheItemPriority.High;

		public void Sanitize()
		{
			AcceptEncodingHeaderKey = AcceptEncodingHeaderKey?.Trim()?.ToLowerInvariant();

			// Clean the paths
			var lstCleanedPath = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			for (int i = 0; i < FrontEndRootFolderAppRelativePaths.Length; i++)
			{
				string path = FrontEndRootFolderAppRelativePaths[i];

				if (string.IsNullOrWhiteSpace(path))
				{
					i--;
					continue;
				}

				path = path.Trim();

				if (path.StartsWith("~"))
					path = path.Remove(0, 1);

				if (!path.StartsWith("/"))
					path = "/" + path;

				lstCleanedPath.Add(path);
			}

			FrontEndRootFolderAppRelativePaths = lstCleanedPath.ToArray();
		}

		public void Validate()
		{
			Guard.ArgumentNotNullOrEmpty(FrontEndRootFolderAppRelativePaths, nameof(FrontEndRootFolderAppRelativePaths));
			Guard.ArgumentNotNullOrEmpty(TargetFileExtensions, nameof(TargetFileExtensions));
			Guard.ArgumentNotNullOrWhiteSpace(AcceptEncodingHeaderKey, nameof(AcceptEncodingHeaderKey));
			Guard.ArgumentInRange(BufferSizeBytes, nameof(BufferSizeBytes), 1);
		}
	}
}