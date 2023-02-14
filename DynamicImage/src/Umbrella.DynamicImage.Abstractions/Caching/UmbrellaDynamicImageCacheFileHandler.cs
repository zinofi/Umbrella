﻿using Microsoft.Extensions.Logging;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Context.Abstractions;

namespace Umbrella.DynamicImage.Abstractions.Caching;

/// <summary>
/// A file handler for accessing files stored in the Dynamic Image cache directory.
/// </summary>
/// <seealso cref="UmbrellaFileHandler{Int32}" />
/// <seealso cref="IUmbrellaDynamicImageCacheFileHandler" />
public class UmbrellaDynamicImageCacheFileHandler : UmbrellaFileHandler<int>, IUmbrellaDynamicImageCacheFileHandler
{
	private readonly DynamicImageCacheCoreOptions _cacheCoreOptions;

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDynamicImageCacheFileHandler"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="cache">The cache.</param>
	/// <param name="cacheKeyUtility">The cache key utility.</param>
	/// <param name="currentUserClaimsPrincipalAccessor">The current user claims principal accessor.</param>
	/// <param name="fileProvider">The file provider.</param>
	/// <param name="options">The options.</param>
	/// <param name="cacheCoreOptions">The cache core options.</param>
	public UmbrellaDynamicImageCacheFileHandler(
		ILogger<UmbrellaDynamicImageCacheFileHandler> logger,
		IHybridCache cache,
		ICacheKeyUtility cacheKeyUtility,
		ICurrentUserClaimsPrincipalAccessor currentUserClaimsPrincipalAccessor,
		IUmbrellaFileStorageProvider fileProvider,
		IUmbrellaFileStorageProviderOptions options,
		DynamicImageCacheCoreOptions cacheCoreOptions)
		: base(logger, cache, cacheKeyUtility, currentUserClaimsPrincipalAccessor, fileProvider, options)
	{
		_cacheCoreOptions = cacheCoreOptions;
	}

	/// <inheritdoc/>
	public override string DirectoryName => _cacheCoreOptions.DirectoryName;

	/// <inheritdoc/>
	public override Task<bool> CanAccessAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default) => Task.FromResult(true);
}