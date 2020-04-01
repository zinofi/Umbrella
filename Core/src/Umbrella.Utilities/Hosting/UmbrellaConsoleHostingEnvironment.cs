using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Constants;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting.Options;

namespace Umbrella.Utilities.Hosting
{
	// TODO: Write unit tests for this and fix issues with paths with slashes, etc.	
	/// <summary>
	/// An implementation of the <see cref="UmbrellaHostingEnvironment" /> type for use with console applications.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Hosting.UmbrellaHostingEnvironment" />
	public class UmbrellaConsoleHostingEnvironment : UmbrellaHostingEnvironment
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaConsoleHostingEnvironment"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="options">The options.</param>
		/// <param name="cache">The cache.</param>
		/// <param name="cacheKeyUtility">The cache key utility.</param>
		public UmbrellaConsoleHostingEnvironment(
			ILogger<UmbrellaConsoleHostingEnvironment> logger,
			UmbrellaConsoleHostingEnvironmentOptions options,
			IMemoryCache cache,
			ICacheKeyUtility cacheKeyUtility)
			: base(logger, options, cache, cacheKeyUtility)
		{
			var fileProviderLazy = new Lazy<IFileProvider>(() => new PhysicalFileProvider(options.BaseDirectory));

			ContentRootFileProvider = fileProviderLazy;
			WebRootFileProvider = fileProviderLazy;
		}

		/// <inheritdoc />
		public override string MapPath(string virtualPath, bool fromContentRoot = true)
		{
			Guard.ArgumentNotNullOrWhiteSpace(virtualPath, nameof(virtualPath));

			if (!fromContentRoot)
				throw new ArgumentException("This value must always be true in a console application.", nameof(fromContentRoot));

			try
			{
				string key = CacheKeyUtility.Create<UmbrellaConsoleHostingEnvironment>(new string[] { virtualPath, fromContentRoot.ToString() });

				return Cache.GetOrCreate(key, entry =>
				{
					entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

					string cleanedPath = TransformPathForFileProvider(virtualPath);

					IFileProvider fileProvider = fromContentRoot ? ContentRootFileProvider.Value : WebRootFileProvider.Value;

					return fileProvider.GetFileInfo(cleanedPath)?.PhysicalPath;
				});
			}
			catch (Exception exc) when (Log.WriteError(exc, new { virtualPath, fromContentRoot }))
			{
				throw new UmbrellaException("There was a problem mapping the path.", exc);
			}
		}

		/// <inheritdoc />
		protected override string TransformPathForFileProvider(string virtualPath)
		{
			ReadOnlySpan<char> span = virtualPath.AsSpan();

			if (span[0] == '~')
				span = span.TrimStart('~');

			if (span[0] != '/')
			{
				int newLength = span.Length + 1;
				Span<char> newSpan = newLength <= StackAllocConstants.MaxCharSize ? stackalloc char[newLength] : new char[newLength];
				newSpan[0] = '/';
				newSpan.Append(1, span);

				return newSpan.ToString();
			}

			return span.ToString();
		}
	}
}