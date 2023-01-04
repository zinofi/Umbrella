// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Caching.Abstractions;

namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// Serves as the base class for file handlers.
/// </summary>
/// <typeparam name="TGroupId">The type of the group id.</typeparam>
/// <typeparam name="TDirectoryType">The type of the directory.</typeparam>
public abstract class UmbrellaFileHandler<TGroupId, TDirectoryType> : IUmbrellaFileHandler<TGroupId>
	where TGroupId : IEquatable<TGroupId>
	where TDirectoryType : struct, Enum
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
	/// Gets the file provider.
	/// </summary>
	protected IUmbrellaFileProvider FileProvider { get; }

	/// <summary>
	/// Gets the file access utility.
	/// </summary>
	protected IUmbrellaFileAccessUtility<TDirectoryType, TGroupId> FileAccessUtility { get; }

	/// <summary>
	/// Gets the type of the directory.
	/// </summary>
	protected abstract TDirectoryType DirectoryType { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaFileHandler{TGroupId, TDirectoryType}"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="cache">The cache.</param>
	/// <param name="cacheKeyUtility">The cache key utility.</param>
	/// <param name="fileProvider">The file provider.</param>
	/// <param name="fileAccessUtility">The file access utility.</param>
	public UmbrellaFileHandler(
		ILogger logger,
		IHybridCache cache,
		ICacheKeyUtility cacheKeyUtility,
		IUmbrellaFileProvider fileProvider,
		IUmbrellaFileAccessUtility<TDirectoryType, TGroupId> fileAccessUtility)
	{
		Logger = logger;
		Cache = cache;
		CacheKeyUtility = cacheKeyUtility;
		FileProvider = fileProvider;
		FileAccessUtility = fileAccessUtility;
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
				IUmbrellaFileInfo? fileInfo = await GetMostRecentExistingFileByGroupIdAsync(groupId, cancellationToken);

				return fileInfo is null ? null : FileAccessUtility.GetWebFilePath(DirectoryType, fileInfo.Name, groupId);
			},
			() => TimeSpan.FromHours(1),
			cancellationToken: cancellationToken);
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
				string filePath = FileAccessUtility.GetFilePath(DirectoryType, providerFileName, groupId);

				IUmbrellaFileInfo? fileInfo = await FileProvider.GetAsync(filePath, cancellationToken);

				return fileInfo is null ? null : FileAccessUtility.GetWebFilePath(DirectoryType, fileInfo.Name, groupId);
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
			string permPath = FileAccessUtility.GetFilePath(DirectoryType, tempFileName, groupId);
			string tempPath = FileAccessUtility.GetTempFilePath(tempFileName);

			IUmbrellaFileInfo? tempFileInfo = await FileProvider.GetAsync(tempPath, cancellationToken);

			if (tempFileInfo is null)
			{
				// It might be the case that the temp file was already moved. Check for it at the permanent path.
				bool exists = await FileProvider.ExistsAsync(permPath, cancellationToken);

				if (exists)
					return FileAccessUtility.GetWebFilePath(DirectoryType, tempFileName, groupId);
			}

			IUmbrellaFileInfo fileInfo = await FileProvider.MoveAsync(tempPath, permPath, cancellationToken);

			await FileAccessUtility.ApplyPermissionsAsync(fileInfo, groupId, true, cancellationToken);

			return FileAccessUtility.GetWebFilePath(DirectoryType, tempFileName, groupId);
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
			string permPath = FileAccessUtility.GetFilePath(DirectoryType, providerFileName, groupId);
			_ = await FileProvider.DeleteAsync(permPath, cancellationToken);

			string key = CacheKeyUtility.Create(GetType(), $"{groupId}:{providerFileName}");
			await Cache.RemoveAsync<string>(key, cancellationToken: cancellationToken);
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
			string directoryName = FileAccessUtility.GetDirectoryName(DirectoryType, groupId);
			await FileProvider.DeleteDirectoryAsync(directoryName, cancellationToken);

			// Remove from the cache
			string key = CacheKeyUtility.Create(GetType(), groupId + "");
			await Cache.RemoveAsync<string>(key, cancellationToken: cancellationToken);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { groupId }))
		{
			throw new UmbrellaFileSystemException("There has been a problem deleting the files for the specified group.", exc);
		}
	}

	private async Task<IUmbrellaFileInfo?> GetMostRecentExistingFileByGroupIdAsync(TGroupId groupId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		string directoryName = FileAccessUtility.GetDirectoryName(DirectoryType, groupId);
		IReadOnlyCollection<IUmbrellaFileInfo> lstFile = await FileProvider.EnumerateDirectoryAsync(directoryName, cancellationToken);

		return lstFile.OrderByDescending(x => x.LastModified).FirstOrDefault();
	}
}