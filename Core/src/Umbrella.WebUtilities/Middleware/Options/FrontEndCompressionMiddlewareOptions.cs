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
	/// <summary>
	/// Options for implementations of the FrontEndCompressionMiddleware in the ASP.NET and ASP.NET Core projects.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Options.CacheableUmbrellaOptions" />
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.ISanitizableUmbrellaOptions" />
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.IValidatableUmbrellaOptions" />
	public class FrontEndCompressionMiddlewareOptions : CacheableUmbrellaOptions, ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the front end root folder application relative paths.
		/// </summary>
		public string[] FrontEndRootFolderAppRelativePaths { get; set; }

		/// <summary>
		/// Gets or sets the target file extensions.
		/// </summary>
		public string[] TargetFileExtensions { get; set; } = new string[] { ".css", ".js" };

		/// <summary>
		/// Gets or sets a value indicating whether to watch the source files.
		/// </summary>
		public bool WatchFiles { get; set; }

		/// <summary>
		/// Gets or sets the Accept-Encoding header key. Defaults to "Accept-Encoding".
		/// This is here to allow the header key to be altered when requests go via a proxy.
		/// </summary>
		public string AcceptEncodingHeaderKey { get; set; } = "Accept-Encoding";

		/// <summary>
		/// Gets or sets a transformation applied to the "Accept-Encoding" header values based on the headers of the current request.
		/// This is useful for last resort scenarios where, e.g. User Agent sniffing is needed to refine the encoding values.
		/// </summary>
		public Action<IReadOnlyDictionary<string, IEnumerable<string>>, HashSet<string>> AcceptEncodingModifier { get; set; }

		/// <summary>
		/// Gets or sets the max-age value of the Cache-Control header. Onlt applicable when <see cref="Cacheability"/> is <see cref="MiddlewareHttpCacheability.Private"/>.
		/// Defaults to 31557600 seconds (1 year - Julian Calendar).
		/// </summary>
		public int? MaxAgeSeconds { get; set; } = 31557600;

		/// <summary>
		/// Gets or sets the cacheability used for the Cache-Control header.
		/// </summary>
		public MiddlewareHttpCacheability Cacheability { get; set; } = MiddlewareHttpCacheability.Private;

		/// <summary>
		/// Gets or sets the must-revalidate value of the Cache-Control header. Only applicable when <see cref="Cacheability"/> is <see cref="MiddlewareHttpCacheability.Private"/>.
		/// Defaults to <see langword="true"/>.
		/// </summary>
		public bool MustRevalidate { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether to apply caching headers to the response. Defaults to <see langword="true"/>.
		/// </summary>
		public bool ResponseCacheEnabled { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether compression is enabled. Defaults to <see langword="true"/>.
		/// </summary>
		public bool CompressionEnabled { get; set; } = true;

		/// <summary>
		/// Gets or sets the buffer size in bytes when copying data between streams during compression. Defaults to 81920.
		/// </summary>
		public int BufferSizeBytes { get; set; } = 81920;

		/// <summary>
		/// Gets or sets the optional response cache determiner. Used to perform additional checks to see if the response should have caching headers applied.
		/// </summary>
		public Func<IFileInfo, bool> ResponseCacheDeterminer { get; set; }

		/// <summary>
		/// Gets or sets the cache priority when caching items in memory. Defaults to <see cref="F:Microsoft.Extensions.Caching.Memory.CacheItemPriority.Normal" />.
		/// </summary>
		public override CacheItemPriority CachePriority { get; set; } = CacheItemPriority.High;

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
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

		/// <summary>
		/// Validates this instance.
		/// </summary>
		/// <exception cref="System.ArgumentException">Public is not allowed. - Cacheability</exception>
		public void Validate()
		{
			Guard.ArgumentNotNullOrEmpty(FrontEndRootFolderAppRelativePaths, nameof(FrontEndRootFolderAppRelativePaths));
			Guard.ArgumentNotNullOrEmpty(TargetFileExtensions, nameof(TargetFileExtensions));
			Guard.ArgumentNotNullOrWhiteSpace(AcceptEncodingHeaderKey, nameof(AcceptEncodingHeaderKey));
			Guard.ArgumentInRange(BufferSizeBytes, nameof(BufferSizeBytes), 1);

			if (Cacheability == MiddlewareHttpCacheability.Public)
				throw new ArgumentException("Public is not allowed.", nameof(Cacheability));

			if (ResponseCacheEnabled && Cacheability == MiddlewareHttpCacheability.NoStore)
				throw new ArgumentException($"{nameof(ResponseCacheEnabled)} is true but {nameof(Cacheability)} is {nameof(MiddlewareHttpCacheability.NoStore)}. This is a contradiction and is not permitted.");
		}
	}
}