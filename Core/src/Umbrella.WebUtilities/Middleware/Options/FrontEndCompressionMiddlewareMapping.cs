using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.Utilities;
using Umbrella.Utilities.Options;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.WebUtilities.Middleware.Options
{
	/// <summary>
	/// Specifies front-end folder path mappings and options for that mapping for use with the front end compression middleware.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Options.CacheableUmbrellaOptions" />
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.ISanitizableUmbrellaOptions" />
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.IValidatableUmbrellaOptions" />
	public class FrontEndCompressionMiddlewareMapping : CacheableUmbrellaOptions, ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the front end root folder application relative paths.
		/// </summary>
		public string[] AppRelativeFolderPaths { get; set; } = null!;

		/// <summary>
		/// Gets or sets the target file extensions. Defaults to .css and .js files.
		/// </summary>
		public string[] TargetFileExtensions { get; set; } = new string[] { ".css", ".js" };

		/// <summary>
		/// Gets or sets a value indicating whether to watch the source files. Defaults to <see langword="false"/>.
		/// </summary>
		public bool WatchFiles { get; set; }

		/// <summary>
		/// Gets or sets the max-age value of the Cache-Control header. Onlt applicable when <see cref="Cacheability"/> is <see cref="MiddlewareHttpCacheability.Private"/>.
		/// Defaults to 31557600 seconds (1 year - Julian Calendar).
		/// </summary>
		public int? MaxAgeSeconds { get; set; } = 31557600;

		/// <summary>
		/// Gets or sets the cacheability used for the Cache-Control header. Defaults to <see cref="MiddlewareHttpCacheability.Private"/>
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
		/// Gets or sets the cache priority when caching items in memory. Defaults to <see cref="CacheItemPriority.High" />.
		/// </summary>
		public override CacheItemPriority CachePriority { get; set; } = CacheItemPriority.High;

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
		public void Sanitize()
		{
			if (AppRelativeFolderPaths != null)
			{
				// Clean the paths
				var lstCleanedPath = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

				for (int i = 0; i < AppRelativeFolderPaths.Length; i++)
				{
					string path = AppRelativeFolderPaths[i];

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

				AppRelativeFolderPaths = lstCleanedPath.ToArray();
			}
		}

		/// <summary>
		/// Validates this instance.
		/// </summary>
		public void Validate()
		{
			Guard.ArgumentNotNullOrEmpty(AppRelativeFolderPaths, nameof(AppRelativeFolderPaths));
			Guard.ArgumentNotNullOrEmpty(TargetFileExtensions, nameof(TargetFileExtensions));

			if (Cacheability == MiddlewareHttpCacheability.Public)
				throw new ArgumentException("Public is not allowed.", nameof(Cacheability));

			if (ResponseCacheEnabled && Cacheability == MiddlewareHttpCacheability.NoStore)
				throw new ArgumentException($"{nameof(ResponseCacheEnabled)} is true but {nameof(Cacheability)} is {nameof(MiddlewareHttpCacheability.NoStore)}. This is a contradiction and is not permitted.");
		}
	}
}