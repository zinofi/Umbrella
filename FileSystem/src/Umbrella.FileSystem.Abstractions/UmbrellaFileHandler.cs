﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching.Abstractions;

namespace Umbrella.FileSystem.Abstractions
{
	/// <summary>
	/// Serves as the base class for file handlers.
	/// </summary>
	/// <typeparam name="TGroupId">The type of the group id.</typeparam>
	/// <typeparam name="TFileAccessUtility">The type of the file access utility.</typeparam>
	/// <typeparam name="TDirectoryType">The type of the directory.</typeparam>
	public abstract class UmbrellaFileHandler<TGroupId, TFileAccessUtility, TDirectoryType> : IUmbrellaFileHandler<TGroupId>
		where TFileAccessUtility : IUmbrellaFileAccessUtility<TDirectoryType, TGroupId>
		where TDirectoryType : struct, Enum
	{
		protected ILogger Logger { get; }
		protected IHybridCache Cache { get; }
		protected ICacheKeyUtility CacheKeyUtility { get; }
		protected IUmbrellaFileProvider FileProvider { get; }
		protected TFileAccessUtility FileAccessUtility { get; }
		protected abstract TDirectoryType DirectoryType { get; }

		public UmbrellaFileHandler(
			ILogger logger,
			IHybridCache cache,
			ICacheKeyUtility cacheKeyUtility,
			IUmbrellaFileProvider fileProvider,
			TFileAccessUtility fileAccessUtility)
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

					return fileInfo is null ? null : FileAccessUtility.GetWebFilePath(DirectoryType, groupId, fileInfo.Name);
				},
				cancellationToken,
				() => TimeSpan.FromHours(1));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { groupId }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem getting the most recent URL.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<string?> GetUrlByGroupIdAndProviderFileNameAsync(TGroupId groupId, string providerFileName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(providerFileName, nameof(providerFileName));

			try
			{
				string key = CacheKeyUtility.Create(GetType(), $"{groupId}:{providerFileName}");

				return await Cache.GetOrCreateAsync(key, async () =>
				{
					string filePath = FileAccessUtility.GetFilePath(DirectoryType, groupId, providerFileName);

					IUmbrellaFileInfo? fileInfo = await FileProvider.GetAsync(filePath, cancellationToken);

					return fileInfo is null ? null : FileAccessUtility.GetWebFilePath(DirectoryType, groupId, fileInfo.Name);
				},
				cancellationToken,
				() => TimeSpan.FromHours(1));
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { groupId, providerFileName }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem getting specified file.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<string> CreateByGroupIdAndTempFileNameAsync(TGroupId groupId, string tempFileName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(tempFileName, nameof(tempFileName));

			try
			{
				// Move the file from the temp folder to the live folder
				string permPath = FileAccessUtility.GetFilePath(DirectoryType, groupId, tempFileName);
				string tempPath = FileAccessUtility.GetTempFilePath(tempFileName);

				IUmbrellaFileInfo? tempFileInfo = await FileProvider.GetAsync(tempPath, cancellationToken);

				if (tempFileInfo is null)
				{
					// It might be the case that the temp file was already moved. Check for it at the permanent path.
					bool exists = await FileProvider.ExistsAsync(permPath, cancellationToken);

					if (exists)
						return FileAccessUtility.GetWebFilePath(DirectoryType, groupId, tempFileName);
				}

				IUmbrellaFileInfo fileInfo = await FileProvider.MoveAsync(tempPath, permPath, cancellationToken);

				await FileAccessUtility.ApplyPermissionsAsync(fileInfo, groupId, cancellationToken);

				return FileAccessUtility.GetWebFilePath(DirectoryType, groupId, tempFileName);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { groupId, tempFileName }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem creating the item.", exc);
			}
		}

		/// <inheritdoc />
		public async Task DeleteByGroupIdAndProviderFileNameAsync(TGroupId groupId, string providerFileName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(providerFileName, nameof(providerFileName));

			try
			{
				string permPath = FileAccessUtility.GetFilePath(DirectoryType, groupId, providerFileName);
				await FileProvider.DeleteAsync(permPath, cancellationToken);

				string key = CacheKeyUtility.Create(GetType(), $"{groupId}:{providerFileName}");
				await Cache.RemoveAsync<string>(key, cancellationToken);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { groupId, providerFileName }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem deleting the item.", exc);
			}
		}

		/// <inheritdoc />
		public async Task DeleteAllByGroupId(TGroupId groupId, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				string directoryName = FileAccessUtility.GetDirectoryName(DirectoryType, groupId);
				await FileProvider.DeleteDirectoryAsync(directoryName, cancellationToken);

				// Remove from the cache
				string key = CacheKeyUtility.Create(GetType(), groupId + "");
				await Cache.RemoveAsync<string>(key, cancellationToken);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { groupId }, returnValue: true))
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
}