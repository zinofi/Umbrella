using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;
using Umbrella.Utilities.Hosting.Options;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.Hosting
{
	public class UmbrellaWebHostingEnvironment : UmbrellaHostingEnvironment, IUmbrellaWebHostingEnvironment
	{
		#region Private Static Members
		private static readonly Regex _multipleForwardSlashRegex = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
		#endregion

		#region Protected Properties
		protected IWebHostEnvironment HostingEnvironment { get; }
		protected IHttpContextAccessor HttpContextAccessor { get; }
		#endregion

		#region Constructors
		public UmbrellaWebHostingEnvironment(ILogger<UmbrellaWebHostingEnvironment> logger,
			IWebHostEnvironment hostingEnvironment,
			IHttpContextAccessor httpContextAccessor,
			UmbrellaHostingEnvironmentOptions options,
			IMemoryCache cache,
			ICacheKeyUtility cacheKeyUtility)
			: base(logger, options, cache, cacheKeyUtility)
		{
			HostingEnvironment = hostingEnvironment;
			HttpContextAccessor = httpContextAccessor;
			ContentRootFileProvider = new Lazy<IFileProvider>(() => HostingEnvironment.ContentRootFileProvider);
			WebRootFileProvider = new Lazy<IFileProvider>(() => HostingEnvironment.WebRootFileProvider);
		}
		#endregion

		#region IUmbrellaHostingEnvironment Members
		public override string MapPath(string virtualPath, bool fromContentRoot = true)
		{
			Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

			string[] cacheKeyParts = null;

			try
			{
				cacheKeyParts = ArrayPool<string>.Shared.Rent(2);
				cacheKeyParts[0] = virtualPath;
				cacheKeyParts[1] = fromContentRoot.ToString();

				string key = CacheKeyUtility.Create<UmbrellaWebHostingEnvironment>(cacheKeyParts, 2);

				return Cache.GetOrCreate(key, entry =>
				{
					entry.SetSlidingExpiration(Options.CacheTimeout).SetPriority(Options.CachePriority);

					// Trim and remove the ~/ from the front of the path
					// Also change forward slashes to back slashes
					string cleanedPath = TransformPath(virtualPath, true, false, true);

					string rootPath = fromContentRoot
						? HostingEnvironment.ContentRootPath
						: HostingEnvironment.WebRootPath;

					return Path.Combine(rootPath, cleanedPath);
				});
			}
			catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, fromContentRoot }, returnValue: true))
			{
				throw new UmbrellaWebException("There has been a problem mapping the specified virtual path.", exc);
			}
			finally
			{
				if (cacheKeyParts != null)
					ArrayPool<string>.Shared.Return(cacheKeyParts);
			}
		}
		#endregion

		#region IUmbrellaWebHostingEnvironment Members
		public virtual string MapWebPath(string virtualPath, bool toAbsoluteUrl = false, string scheme = "http", bool appendVersion = false, string versionParameterName = "v", bool mapFromContentRoot = true, bool watchWhenAppendVersion = true)
		{
			Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));
			Guard.ArgumentNotNullOrWhiteSpace(scheme, nameof(scheme));
			Guard.ArgumentNotNullOrWhiteSpace(versionParameterName, nameof(versionParameterName));

			string[] cacheKeyParts = null;

			try
			{
				cacheKeyParts = ArrayPool<string>.Shared.Rent(7);
				cacheKeyParts[0] = virtualPath;
				cacheKeyParts[1] = toAbsoluteUrl.ToString();
				cacheKeyParts[2] = scheme;
				cacheKeyParts[3] = appendVersion.ToString();
				cacheKeyParts[4] = versionParameterName;
				cacheKeyParts[5] = mapFromContentRoot.ToString();
				cacheKeyParts[6] = watchWhenAppendVersion.ToString();

				string key = CacheKeyUtility.Create<UmbrellaWebHostingEnvironment>(cacheKeyParts, 7);

				return Cache.GetOrCreate(key, entry =>
				{
					entry.SetSlidingExpiration(Options.CacheTimeout).SetPriority(Options.CachePriority);

					string cleanedPath = TransformPathForFileProvider(virtualPath);

					PathString applicationPath = HttpContextAccessor.HttpContext.Request.PathBase;

					PathString virtualApplicationPath = applicationPath != "/"
						? applicationPath
						: PathString.Empty;

					// Prefix the path with the virtual application segment but only if the cleanedPath doesn't already start with the segment
					string url = cleanedPath.StartsWith(virtualApplicationPath, StringComparison.OrdinalIgnoreCase)
						? cleanedPath
						: virtualApplicationPath.Add(cleanedPath).Value;

					if (toAbsoluteUrl)
						url = $"{scheme}://{ResolveHttpHost()}{url}";

					if (appendVersion)
					{
						IFileProvider fileProvider = mapFromContentRoot
							? ContentRootFileProvider.Value
							: WebRootFileProvider.Value;

						IFileInfo fileInfo = fileProvider.GetFileInfo(cleanedPath);

						if (!fileInfo.Exists)
							throw new FileNotFoundException($"The specified virtual path {virtualPath} does not exist on disk at {fileInfo.PhysicalPath}.");

						if (watchWhenAppendVersion)
							entry.AddExpirationToken(fileProvider.Watch(cleanedPath));

						long versionHash = fileInfo.LastModified.UtcDateTime.ToFileTimeUtc() ^ fileInfo.Length;
						string version = Convert.ToString(versionHash, 16);

						string qsStart = url.Contains("?") ? "&" : "?";

						url = $"{url}{qsStart}{versionParameterName}={version}";
					}

					return url;
				});
			}
			catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, toAbsoluteUrl, scheme, appendVersion, versionParameterName, mapFromContentRoot }))
			{
				throw;
			}
		}
		#endregion

		#region Protected Methods
		protected virtual string ResolveHttpHost() => HttpContextAccessor.HttpContext.Request.Host.Value;
		protected override string TransformPathForFileProvider(string virtualPath) => TransformPath(virtualPath, false, true, false);
		#endregion

		#region Internal Methods
		internal string TransformPath(string virtualPath, bool removeLeadingSlash, bool ensureLeadingSlash, bool convertForwardSlashesToBackSlashes)
		{
			StringBuilder sb = new StringBuilder(virtualPath)
				.Trim()
				.Trim('~');

			if (removeLeadingSlash && ensureLeadingSlash)
				throw new ArgumentException($"{nameof(removeLeadingSlash)} and {nameof(ensureLeadingSlash)} are both set to true. This is not allowed.");

			if (removeLeadingSlash)
				sb.Trim('/');

			if (ensureLeadingSlash && !sb.StartsWith('/'))
				sb.Insert(0, '/');

			string path = _multipleForwardSlashRegex.Replace(sb.ToString(), "/");

			if (convertForwardSlashesToBackSlashes)
				path = path.Replace("/", @"\");

			return path;
		}
		#endregion
	}
}