using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities.Caching.Abstractions;

namespace Umbrella.DynamicImage.Abstractions.Caching
{
	/// <summary>
	/// Serves as the base class for caches that store files on a physical medium, e.g. Disk, Azure storage, etc.
	/// </summary>
	/// <typeparam name="TFileProvider">The type of the file provider.</typeparam>
	/// <seealso cref="DynamicImageCache" />
	/// <seealso cref="IDynamicImageCache" />
	public abstract class DynamicImagePhysicalCache<TFileProvider> : DynamicImageCache, IDynamicImageCache
		where TFileProvider : IUmbrellaFileProvider
	{
		#region Public Properties		
		/// <summary>
		/// Gets the cache path format.
		/// </summary>
		public virtual string CachePathFormat => $"{Path.DirectorySeparatorChar}{{0}}.{{1}}";
		#endregion

		#region Protected Properties		
		/// <summary>
		/// Gets the file provider.
		/// </summary>
		protected TFileProvider FileProvider { get; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicImagePhysicalCache{TFileProvider}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="cache">The cache.</param>
		/// <param name="cacheKeyUtility">The cache key utility.</param>
		/// <param name="cacheOptions">The cache options.</param>
		/// <param name="fileProvider">The file provider.</param>
		public DynamicImagePhysicalCache(
			ILogger logger,
			IHybridCache cache,
			ICacheKeyUtility cacheKeyUtility,
			DynamicImageCacheCoreOptions cacheOptions,
			TFileProvider fileProvider)
			: base(logger, cache, cacheKeyUtility, cacheOptions)
		{
			FileProvider = fileProvider;
		}
		#endregion

		#region IDynamicImageCache Members
		/// <inheritdoc />
		public virtual async Task AddAsync(DynamicImageItem dynamicImage, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();

				string key = GenerateCacheKey(dynamicImage.ImageOptions);

				//Save to disk - overwrite any existing entry
				string subPath = GetSubPath(key, dynamicImage.ImageOptions.Format.ToFileExtensionString());

				//Read the image content to save to the underlying file store
				byte[] bytes = await dynamicImage.GetContentAsync(cancellationToken).ConfigureAwait(false);

				await FileProvider.SaveAsync(subPath, bytes, false, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { dynamicImage.ImageOptions }, returnValue: true))
			{
				throw new DynamicImageException($"There was a problem adding the {nameof(DynamicImageItem)} to the cache.", exc, dynamicImage.ImageOptions);
			}
		}

		/// <inheritdoc />
		public virtual async Task<DynamicImageItem> GetAsync(DynamicImageOptions options, DateTimeOffset sourceLastModified, string fileExtension, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();

				string cacheKey = GenerateCacheKey(options);

				string subPath = GetSubPath(cacheKey, fileExtension);

				//Find the cached file
				IUmbrellaFileInfo fileInfo = await FileProvider.GetAsync(subPath, cancellationToken).ConfigureAwait(false);

				//No cached image available
				if (fileInfo == null)
					return null;

				//If the file does not exist or has been modified since the item was generated,
				//evict it from the cache, i.e. delete the cached image from disk
				if (sourceLastModified > fileInfo.LastModified)
				{
					await fileInfo.DeleteAsync(cancellationToken).ConfigureAwait(false);

					return null;
				}

				//We need to return the cached image
				var item = new DynamicImageItem
				{
					UmbrellaFileInfo = fileInfo,
					ImageOptions = options
				};

				return item;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { options, sourceLastModified, fileExtension }, returnValue: true))
			{
				throw new DynamicImageException("There was problem retrieving the image from the cache.", exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task RemoveAsync(DynamicImageOptions options, string fileExtension, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();

				string cacheKey = GenerateCacheKey(options);

				string subPath = GetSubPath(cacheKey, fileExtension);
				await FileProvider.DeleteAsync(subPath).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { options, fileExtension }, returnValue: true))
			{
				throw new DynamicImageException("There was problem removing the image from the cache.", exc);
			}
		}
		#endregion

		#region Overridden Methods
		/// <inheritdoc />
		protected override string GetSubPath(string cacheKey, string fileExtension)
			=> $@"{Path.DirectorySeparatorChar}{cacheKey}.{fileExtension}";
		#endregion
	}
}