using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Constants;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;
using Umbrella.Utilities.Hosting.Options;
using Umbrella.Utilities.Primitives;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.Hosting
{
	public class UmbrellaWebHostingEnvironment : UmbrellaHostingEnvironment, IUmbrellaWebHostingEnvironment
	{
		#region Constructors
		public UmbrellaWebHostingEnvironment(ILogger<UmbrellaWebHostingEnvironment> logger,
			UmbrellaHostingEnvironmentOptions options,
			IMemoryCache cache,
			ICacheKeyUtility cacheKeyUtility)
			: base(logger, options, cache, cacheKeyUtility)
		{
			var fileProviderLazy = new Lazy<IFileProvider>(() => new PhysicalFileProvider(MapPath("~/")));

			ContentRootFileProvider = fileProviderLazy;
			WebRootFileProvider = fileProviderLazy;
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

					if (virtualPath == "~/")
						return System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);

					string cleanedPath = TransformPathForFileProvider(virtualPath);

					IFileProvider fileProvider = fromContentRoot ? ContentRootFileProvider.Value : WebRootFileProvider.Value;

					return fileProvider.GetFileInfo(cleanedPath)?.PhysicalPath;
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

					string cleanedPath = TransformPath(virtualPath, false, true, true);

					string virtualApplicationPath = HttpRuntime.AppDomainAppVirtualPath != "/"
						? HttpRuntime.AppDomainAppVirtualPath
						: "";

					//Prefix the path with the virtual application segment but only if the cleanedPath doesn't already start with the segment
					string url = cleanedPath.StartsWith(virtualApplicationPath, StringComparison.OrdinalIgnoreCase)
						? cleanedPath
						: virtualApplicationPath + cleanedPath;

					if (toAbsoluteUrl)
						url = $"{scheme}://{ResolveHttpHost()}{url}";

					if (appendVersion)
					{
						string physicalPath = MapPath(cleanedPath, mapFromContentRoot);

						if(string.IsNullOrWhiteSpace(physicalPath))
							throw new FileNotFoundException($"The specified virtual path {virtualPath} does not exist on disk at {physicalPath}.");

						var fileInfo = new FileInfo(physicalPath);

						if (!fileInfo.Exists)
							throw new FileNotFoundException($"The specified virtual path {virtualPath} does not exist on disk at {physicalPath}.");

						if (watchWhenAppendVersion)
							entry.AddExpirationToken(new PhysicalFileChangeToken(fileInfo));

						long versionHash = fileInfo.LastWriteTimeUtc.ToFileTimeUtc() ^ fileInfo.Length;
						string version = Convert.ToString(versionHash, 16);

						string qsStart = url.Contains("?") ? "&" : "?";

						url = $"{url}{qsStart}{versionParameterName}={version}";
					}

					return url;
				});
			}
			catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, toAbsoluteUrl, scheme, appendVersion, versionParameterName, mapFromContentRoot, watchWhenAppendVersion }, returnValue: true))
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

		#region Protected Methods
		protected virtual string ResolveHttpHost() => HttpContext.Current.Request.Url.Host;
		protected override string TransformPathForFileProvider(string virtualPath) => TransformPath(virtualPath, false, true, true);
		#endregion

		#region Internal Methods
		internal string TransformPath(string virtualPath, bool ensureStartsWithTildeSlash, bool ensureNoTilde, bool ensureLeadingSlash)
		{
			ReadOnlySpan<char> span = virtualPath.AsSpan().Trim();

			int length = span.Length;

			if (ensureNoTilde && span[0] == '~')
				span = span.TrimStart('~');

			bool leadingSlashToInsert = false;

			if (ensureLeadingSlash && span[0] != '/')
				leadingSlashToInsert = true;

			bool leadingTildaToInsert = false;
			bool leadingTildaSlashToInsert = false;

			if (ensureStartsWithTildeSlash && !span.StartsWith("~/".AsSpan(), StringComparison.Ordinal))
			{
				if (span[0] == '/')
					leadingTildaToInsert = true;
				else
					leadingTildaSlashToInsert = true;
			}

			int duplicateSlashCount = 0;

			if (leadingTildaSlashToInsert)
			{
				int newLength = span.Length + 2;

				Span<char> outputSpan = newLength <= StackAllocConstants.MaxCharSize ? stackalloc char[newLength] : new char[newLength];
				outputSpan[0] = '~';
				outputSpan[1] = '/';
				span.CopyTo(outputSpan.Slice(2));

				for (int i = 0; i < outputSpan.Length; i++)
				{
					if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
					{
						duplicateSlashCount++;
					}
				}

				if (duplicateSlashCount > 0)
				{
					int duplicateSlashCountNewLength = outputSpan.Length - duplicateSlashCount;

					Span<char> cleanedSpan = duplicateSlashCountNewLength <= StackAllocConstants.MaxCharSize ? stackalloc char[duplicateSlashCountNewLength] : new char[duplicateSlashCountNewLength];

					int j = 0;
					for (int i = 0; i < outputSpan.Length; i++)
					{
						if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
						{
							continue;
						}

						cleanedSpan[j++] = outputSpan[i];
					}

					return cleanedSpan.ToString();
				}

				return outputSpan.ToString();
			}
			else if (leadingTildaToInsert)
			{
				int newLength = span.Length + 1;

				Span<char> outputSpan = newLength <= StackAllocConstants.MaxCharSize ? stackalloc char[newLength] : new char[newLength];
				outputSpan[0] = '~';
				span.CopyTo(outputSpan.Slice(1));

				for (int i = 0; i < outputSpan.Length; i++)
				{
					if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
					{
						duplicateSlashCount++;
					}
				}

				if (duplicateSlashCount > 0)
				{
					int duplicateSlashCountNewLength = outputSpan.Length - duplicateSlashCount;

					Span<char> cleanedSpan = duplicateSlashCountNewLength <= StackAllocConstants.MaxCharSize ? stackalloc char[duplicateSlashCountNewLength] : new char[duplicateSlashCountNewLength];

					int j = 0;
					for (int i = 0; i < outputSpan.Length; i++)
					{
						if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
						{
							continue;
						}

						cleanedSpan[j++] = outputSpan[i];
					}

					return cleanedSpan.ToString();
				}

				return outputSpan.ToString();
			}
			else if (leadingSlashToInsert)
			{
				int newLength = span.Length + 1;

				Span<char> outputSpan = newLength <= StackAllocConstants.MaxCharSize ? stackalloc char[newLength] : new char[newLength];
				outputSpan[0] = '/';
				span.CopyTo(outputSpan.Slice(1));

				for (int i = 0; i < outputSpan.Length; i++)
				{
					if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
					{
						duplicateSlashCount++;
					}
				}

				if (duplicateSlashCount > 0)
				{
					int duplicateSlashCountNewLength = outputSpan.Length - duplicateSlashCount;

					Span<char> cleanedSpan = duplicateSlashCountNewLength <= StackAllocConstants.MaxCharSize ? stackalloc char[duplicateSlashCountNewLength] : new char[duplicateSlashCountNewLength];

					int j = 0;
					for (int i = 0; i < outputSpan.Length; i++)
					{
						if (i > 0 && outputSpan[i] == '/' && outputSpan[i - 1] == '/')
						{
							continue;
						}

						cleanedSpan[j++] = outputSpan[i];
					}

					return cleanedSpan.ToString();
				}

				return outputSpan.ToString();
			}

			for (int i = 0; i < span.Length; i++)
			{
				if (i > 0 && span[i] == '/' && span[i - 1] == '/')
				{
					duplicateSlashCount++;
				}
			}

			if (duplicateSlashCount > 0)
			{
				int duplicateSlashCountNewLength = span.Length - duplicateSlashCount;

				Span<char> cleanedSpan = duplicateSlashCountNewLength <= StackAllocConstants.MaxCharSize ? stackalloc char[duplicateSlashCountNewLength] : new char[duplicateSlashCountNewLength];

				int j = 0;
				for (int i = 0; i < span.Length; i++)
				{
					if (i > 0 && span[i] == '/' && span[i - 1] == '/')
					{
						continue;
					}

					cleanedSpan[j++] = span[i];
				}

				return cleanedSpan.ToString();
			}

			return span.ToString();
		}

		[Obsolete]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal string TransformPathOld(string virtualPath, bool ensureStartsWithTildeSlash, bool ensureNoTilde, bool ensureLeadingSlash)
		{
			StringBuilder sb = new StringBuilder(virtualPath)
				.Trim();

			if (ensureNoTilde && sb.StartsWith("~"))
				sb.Remove(0, 1);

			if (ensureLeadingSlash && !sb.StartsWith('/'))
				sb.Insert(0, '/');

			if (ensureStartsWithTildeSlash && !sb.StartsWith("~/"))
			{
				if (sb.StartsWith('/'))
					sb.Insert(0, '~');
				else
					sb.Insert(0, "~/");
			}

			for (int i = 0; i < sb.Length; i++)
			{
				if (i > 0 && sb[i] == '/' && sb[i - 1] == '/')
					sb.Remove(i--, 1);
			}

			return sb.ToString();
		}
		#endregion
	}
}