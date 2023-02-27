// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Context.Abstractions;
using Umbrella.Utilities.Security.Extensions;

namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// Serves as the base class for file handlers.
/// </summary>
/// <typeparam name="TGroupId">The type of the group id.</typeparam>
public abstract class UmbrellaFileHandler<TGroupId> : IUmbrellaFileHandler<TGroupId>
	where TGroupId : IEquatable<TGroupId>
{
	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets the cache.
	/// </summary>
	protected IHybridCache Cache { get; }

	/// <summary>
	/// Gets the cache key utility.
	/// </summary>
	protected ICacheKeyUtility CacheKeyUtility { get; }

	/// <summary>
	/// Gets the current user claims principal accessor.
	/// </summary>
	public ICurrentUserClaimsPrincipalAccessor CurrentUserClaimsPrincipalAccessor { get; }

	/// <summary>
	/// Gets the file provider.
	/// </summary>
	protected IUmbrellaFileStorageProvider FileProvider { get; }

	/// <summary>
	/// Gets the options.
	/// </summary>
	public IUmbrellaFileStorageProviderOptions Options { get; }

	/// <inheritdoc/>
	public abstract string DirectoryName { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaFileHandler{TGroupId}"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="cache">The cache.</param>
	/// <param name="cacheKeyUtility">The cache key utility.</param>
	/// <param name="currentUserClaimsPrincipalAccessor">The current user claims principal accessor.</param>
	/// <param name="fileProvider">The file provider.</param>
	/// <param name="options">The options.</param>
	public UmbrellaFileHandler(
		ILogger logger,
		IHybridCache cache,
		ICacheKeyUtility cacheKeyUtility,
		ICurrentUserClaimsPrincipalAccessor currentUserClaimsPrincipalAccessor,
		IUmbrellaFileStorageProvider fileProvider,
		IUmbrellaFileStorageProviderOptions options)
	{
		Logger = logger;
		Cache = cache;
		CacheKeyUtility = cacheKeyUtility;
		CurrentUserClaimsPrincipalAccessor = currentUserClaimsPrincipalAccessor;
		FileProvider = fileProvider;
		Options = options;
	}

	/// <inheritdoc />
	public async Task<string?> GetMostRecentUrlByGroupIdAsync(TGroupId groupId, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			string key = CacheKeyUtility.Create(GetType(), groupId + "");

			return await Cache.GetOrCreateAsync(key, async () =>
			{
				IUmbrellaFileInfo? fileInfo = await GetMostRecentExistingFileByGroupIdAsync(groupId, cancellationToken).ConfigureAwait(false);

				return fileInfo is null ? null : GetWebFilePath(fileInfo.Name, groupId);
			},
			() => TimeSpan.FromHours(1),
			cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { groupId }))
		{
			throw new UmbrellaFileSystemException("There has been a problem getting the most recent URL.", exc);
		}
	}

	/// <inheritdoc />
	public async Task<string?> GetUrlByGroupIdAndProviderFileNameAsync(TGroupId groupId, string providerFileName, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(providerFileName);

		try
		{
			string key = CacheKeyUtility.Create(GetType(), $"{groupId}:{providerFileName}");

			return await Cache.GetOrCreateAsync(key, async () =>
			{
				string filePath = GetFilePath(providerFileName, groupId);

				IUmbrellaFileInfo? fileInfo = await FileProvider.GetAsync(filePath, cancellationToken).ConfigureAwait(false);

				return fileInfo is null ? null : GetWebFilePath(fileInfo.Name, groupId);
			},
			() => TimeSpan.FromHours(1),
			cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { groupId, providerFileName }))
		{
			throw new UmbrellaFileSystemException("There has been a problem getting specified file.", exc);
		}
	}

	/// <inheritdoc />
	public async Task<string> CreateByGroupIdAndTempFileNameAsync(TGroupId groupId, string tempFileName, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(tempFileName);

		try
		{
			// Move the file from the temp folder to the live folder
			string permPath = GetFilePath(tempFileName, groupId);
			string tempPath = GetTempFilePath(tempFileName);

			IUmbrellaFileInfo? tempFileInfo = await FileProvider.GetAsync(tempPath, cancellationToken).ConfigureAwait(false);

			if (tempFileInfo is null)
			{
				// It might be the case that the temp file was already moved. Check for it at the permanent path.
				bool exists = await FileProvider.ExistsAsync(permPath, cancellationToken).ConfigureAwait(false);

				if (exists)
					return GetWebFilePath(tempFileName, groupId);
			}

			IUmbrellaFileInfo fileInfo = await FileProvider.MoveAsync(tempPath, permPath, cancellationToken).ConfigureAwait(false);

			await ApplyPermissionsAsync(fileInfo, groupId, true, cancellationToken).ConfigureAwait(false);

			return GetWebFilePath(tempFileName, groupId);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { groupId, tempFileName }))
		{
			throw new UmbrellaFileSystemException("There has been a problem creating the item.", exc);
		}
	}

	/// <inheritdoc />
	public async Task DeleteByGroupIdAndProviderFileNameAsync(TGroupId groupId, string providerFileName, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(providerFileName);

		try
		{
			string permPath = GetFilePath(providerFileName, groupId);
			_ = await FileProvider.DeleteAsync(permPath, cancellationToken).ConfigureAwait(false);

			string key = CacheKeyUtility.Create(GetType(), $"{groupId}:{providerFileName}");
			await Cache.RemoveAsync<string>(key, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { groupId, providerFileName }))
		{
			throw new UmbrellaFileSystemException("There has been a problem deleting the item.", exc);
		}
	}

	/// <inheritdoc />
	public async Task DeleteAllByGroupIdAsync(TGroupId groupId, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			string directoryName = GetDirectoryName(groupId);
			await FileProvider.DeleteDirectoryAsync(directoryName, cancellationToken).ConfigureAwait(false);

			// Remove from the cache
			string key = CacheKeyUtility.Create(GetType(), groupId + "");
			await Cache.RemoveAsync<string>(key, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { groupId }))
		{
			throw new UmbrellaFileSystemException("There has been a problem deleting the files for the specified group.", exc);
		}
	}

	/// <inheritdoc />
	public abstract Task<bool> CanAccessAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default);

	/// <inheritdoc />
	public virtual async Task ApplyPermissionsAsync(IUmbrellaFileInfo fileInfo, TGroupId groupId, bool writeChanges = true, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			if (CurrentUserClaimsPrincipalAccessor.CurrentPrincipal is not null)
				await fileInfo.SetCreatedByIdAsync(CurrentUserClaimsPrincipalAccessor.CurrentPrincipal.GetId<string>(), false, cancellationToken).ConfigureAwait(false);

			if (writeChanges)
				await fileInfo.WriteMetadataChangesAsync(cancellationToken).ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { fileInfo.SubPath, groupId, writeChanges }))
		{
			throw new UmbrellaFileSystemException("There has been a problem applying the required file permissions.", exc);
		}
	}

	/// <inheritdoc />
	public async Task<IUmbrellaFileInfo> SaveAsync(TGroupId groupId, string fileName, byte[] bytes, bool cacheContents = true, int? bufferSizeOverride = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			string subPath = GetFilePath(fileName, groupId);
			
			return await FileProvider.SaveAsync(subPath, bytes, cacheContents, bufferSizeOverride, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { groupId, fileName, cacheContents, bufferSizeOverride }))
		{
			throw new UmbrellaFileSystemException("There has been a problem saving the file.", exc);
		}
	}

	/// <inheritdoc />
	public async Task<IUmbrellaFileInfo?> GetAsync(TGroupId groupId, string fileName, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			string subPath = GetFilePath(fileName, groupId);

			return await FileProvider.GetAsync(subPath, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { groupId, fileName }))
		{
			throw new UmbrellaFileSystemException("There has been a problem saving the file.", exc);
		}
	}

	private async Task<IUmbrellaFileInfo?> GetMostRecentExistingFileByGroupIdAsync(TGroupId groupId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		string directoryName = GetDirectoryName(groupId);
		IReadOnlyCollection<IUmbrellaFileInfo> lstFile = await FileProvider.EnumerateDirectoryAsync(directoryName, cancellationToken).ConfigureAwait(false);

		return lstFile.OrderByDescending(x => x.LastModified).FirstOrDefault();
	}

	/// <inheritdoc/>
	public string GetTempDirectoryName() => $"/{Options.TempFilesDirectoryName}";

	/// <inheritdoc/>
	public string GetTempFilePath(string fileName) => $"{GetTempDirectoryName()}/{fileName}";

	/// <inheritdoc/>
	public string GetTempWebFilePath(string fileName) => $"/{Options.WebFilesDirectoryName}{GetTempFilePath(fileName)}".ToLowerInvariant();

	/// <inheritdoc/>
	public bool IsTempFilePath(string filePath) => filePath.StartsWith(GetTempDirectoryName() + "/", StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc/>
	public string GetDirectoryName(TGroupId groupId) => $"/{DirectoryName}/{groupId}";

	/// <inheritdoc/>
	public string GetFilePath(string fileName, TGroupId groupId) => $"{GetDirectoryName(groupId)}/{fileName}";

	/// <inheritdoc/>
	public string GetWebFilePath(string fileName, TGroupId groupId) => $"/{Options.WebFilesDirectoryName}{GetFilePath(fileName, groupId)}".ToLowerInvariant();
}