using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.AzureStorage.Extensions;
using Umbrella.Utilities;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.AzureStorage
{
	/// <summary>
	/// An implementation of <see cref="UmbrellaFileProvider{TFileInfo, TOptions}"/> which uses Azure Blob Storage as the underlying storage mechanism.
	/// </summary>
	/// <seealso cref="T:Umbrella.FileSystem.AzureStorage.UmbrellaAzureBlobStorageFileProvider{Umbrella.FileSystem.AzureStorage.UmbrellaAzureBlobStorageFileProviderOptions}" />
	public class UmbrellaAzureBlobStorageFileProvider : UmbrellaAzureBlobStorageFileProvider<UmbrellaAzureBlobStorageFileProviderOptions>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaAzureBlobStorageFileProvider"/> class.
		/// </summary>
		/// <param name="loggerFactory">The logger factory.</param>
		/// <param name="mimeTypeUtility">The MIME type utility.</param>
		/// <param name="genericTypeConverter">The generic type converter.</param>
		public UmbrellaAzureBlobStorageFileProvider(
			ILoggerFactory loggerFactory,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter)
			: base(loggerFactory, mimeTypeUtility, genericTypeConverter)
		{
		}
	}

	/// <summary>
	/// An implementation of <see cref="UmbrellaFileProvider{TFileInfo, TOptions}"/> which uses Azure Blob Storage as the underlying storage mechanism.
	/// </summary>
	/// <typeparam name="TOptions">The type of the provider options.</typeparam>
	/// <seealso cref="T:Umbrella.FileSystem.AzureStorage.UmbrellaAzureBlobStorageFileProvider{Umbrella.FileSystem.AzureStorage.UmbrellaAzureBlobStorageFileProviderOptions}" />
	public class UmbrellaAzureBlobStorageFileProvider<TOptions> : UmbrellaFileProvider<UmbrellaAzureBlobStorageFileInfo, UmbrellaAzureBlobStorageFileProviderOptions>, IUmbrellaAzureBlobStorageFileProvider, IDisposable
		where TOptions : UmbrellaAzureBlobStorageFileProviderOptions
	{
		#region Constants
		private const string DirectorySeparator = "/";
		#endregion

		#region Private Static Members
		private static readonly char[] _directorySeparatorArray = new[] { '/' };
		#endregion

		#region Private Members
		private readonly SemaphoreSlim _containerCacheLock = new SemaphoreSlim(1, 1);
		#endregion

		#region Protected Properties
		/// <summary>
		/// Gets the service client.
		/// </summary>
		protected BlobServiceClient ServiceClient { get; set; }

		/// <summary>
		/// Gets the container resolution cache.
		/// </summary>
		protected ConcurrentDictionary<string, bool> ContainerResolutionCache { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaAzureBlobStorageFileProvider{TOptions}"/> class.
		/// </summary>
		/// <param name="loggerFactory">The logger factory.</param>
		/// <param name="mimeTypeUtility">The MIME type utility.</param>
		/// <param name="genericTypeConverter">The generic type converter.</param>
		public UmbrellaAzureBlobStorageFileProvider(
			ILoggerFactory loggerFactory,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter)
			: base(loggerFactory.CreateLogger<UmbrellaAzureBlobStorageFileProvider>(), loggerFactory, mimeTypeUtility, genericTypeConverter)
		{
		}
		#endregion

		#region IUmbrellaAzureBlobStorageFileProvider Members
		/// <inheritdoc />
		/// <exception cref="UmbrellaFileSystemException">There has been a problem clearing the container resolution cache.</exception>
		public void ClearContainerResolutionCache()
		{
			try
			{
				ContainerResolutionCache?.Clear();
			}
			catch (Exception exc) when (Log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem clearing the container resolution cache.", exc);
			}
		}
		#endregion

		/// <inheritdoc />
		public async Task DeleteDirectoryAsync(string subpath, CancellationToken cancellationToken = default)
		{
			try
			{
				string cleanedPath = SanitizeSubPathCore(subpath);

				if (Log.IsEnabled(LogLevel.Debug))
					Log.WriteDebug(new { subpath, cleanedPath }, "Directory");

				string[] parts = cleanedPath.Split(_directorySeparatorArray, StringSplitOptions.RemoveEmptyEntries);

				string containerName = NormalizeContainerName(parts[0]);

				BlobContainerClient container = ServiceClient.GetBlobContainerClient(containerName);

				if (!await container.ExistsAsync(cancellationToken).ConfigureAwait(false))
					return;

				if (parts.Length == 1)
				{
					// Just delete the container
					await container.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

					// Remove from the resolution cache.
					ContainerResolutionCache.TryRemove(containerName, out bool success);
				}
				else
				{
					List<BlobClient> lstBlob = await container.GetBlobsByDirectoryAsync(string.Join(DirectorySeparator, parts.Skip(1)), cancellationToken, false).ConfigureAwait(false);

					foreach (BlobClient blob in lstBlob)
					{
						await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken).ConfigureAwait(false);
					}
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem deleting the specified directory.", exc);
			}
		}

		/// <inheritdoc />
		public async Task<IReadOnlyCollection<IUmbrellaFileInfo>> EnumerateDirectoryAsync(string subpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			try
			{
				string cleanedPath = SanitizeSubPathCore(subpath);

				if (Log.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
					Log.WriteDebug(new { subpath, cleanedPath }, "Directory");

				string[] parts = cleanedPath.Split(_directorySeparatorArray, StringSplitOptions.RemoveEmptyEntries);

				string containerName = NormalizeContainerName(parts[0]);

				BlobContainerClient container = ServiceClient.GetBlobContainerClient(containerName);

				if (!await container.ExistsAsync(cancellationToken).ConfigureAwait(false))
					return Array.Empty<IUmbrellaFileInfo>();

				List<BlobClient> lstBlob = await container.GetBlobsByDirectoryAsync(string.Join(DirectorySeparator, parts.Skip(1)), cancellationToken).ConfigureAwait(false);

				UmbrellaAzureBlobStorageFileInfo[] files = lstBlob.Select(x => new UmbrellaAzureBlobStorageFileInfo(FileInfoLoggerInstance, MimeTypeUtility, GenericTypeConverter, $"/{parts[0]}/{x.Name}", this, x, false)).ToArray();

				var lstResult = new List<UmbrellaAzureBlobStorageFileInfo>();

				foreach (var file in files)
				{
					await file.InitializeAsync(cancellationToken).ConfigureAwait(false);

					if (await CheckFileAccessAsync(file, file.Blob, cancellationToken).ConfigureAwait(false))
						lstResult.Add(file);
					else
						Log.WriteWarning(state: new { file.SubPath }, message: "File access failed.");
				}

				return lstResult;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem enumerating the files in the specified directory.", exc);
			}
		}

		#region Overridden Methods
		/// <inheritdoc />
		public override void InitializeOptions(IUmbrellaFileProviderOptions options)
		{
			base.InitializeOptions(options);

			ServiceClient = new BlobServiceClient(Options.StorageConnectionString);

			if (Options.CacheContainerResolutions)
				ContainerResolutionCache = new ConcurrentDictionary<string, bool>();
		}

		/// <inheritdoc />
		protected override async Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			string cleanedPath = SanitizeSubPathCore(subpath);

			if (Log.IsEnabled(LogLevel.Debug))
				Log.WriteDebug(new { subpath, cleanedPath }, "File");

			string[] parts = cleanedPath.Split(_directorySeparatorArray, StringSplitOptions.RemoveEmptyEntries);

			string containerName = NormalizeContainerName(parts[0]);
			string blobName = string.Join("/", parts.Skip(1));

			// TODO: Need to create a regex to replace what this code does. Try and find the original regexes in the github repo.
			// NameValidator.ValidateContainerName(containerName);
			// NameValidator.ValidateBlobName(blobName);

			BlobContainerClient container = ServiceClient.GetBlobContainerClient(containerName);

			if (ContainerResolutionCache != null && !ContainerResolutionCache.ContainsKey(containerName))
			{
				await _containerCacheLock.WaitAsync(cancellationToken).ConfigureAwait(false);

				try
				{
					if (ContainerResolutionCache != null && !ContainerResolutionCache.ContainsKey(containerName))
					{
						await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

						// The value can be anything here but we need to use ConcurrentDictioary because there isn't a ConcurrentHashSet type.
						// Could implement our own locking mechanism around a HashSet but not worth it. Maybe consider in the future or see TODO above.
						ContainerResolutionCache.TryAdd(containerName, true);
					}
				}
				finally
				{
					_containerCacheLock.Release();
				}
			}

			BlobClient blob = container.GetBlobClient(blobName);

			// The call to ExistsAsync should force the properties of the blob to be populated
			if (!isNew && !await blob.ExistsAsync(cancellationToken).ConfigureAwait(false))
				return null;

			var fileInfo = new UmbrellaAzureBlobStorageFileInfo(FileInfoLoggerInstance, MimeTypeUtility, GenericTypeConverter, subpath, this, blob, isNew);
			await fileInfo.InitializeAsync(cancellationToken).ConfigureAwait(false);

			if (!await CheckFileAccessAsync(fileInfo, blob, cancellationToken).ConfigureAwait(false))
				throw new UmbrellaFileAccessDeniedException(subpath);

			return fileInfo;
		}
		#endregion

		#region Private Methods
		private string NormalizeContainerName(string containerName) => containerName.Trim().ToLowerInvariant();
		#endregion

		#region Protected Methods
		/// <summary>
		/// Performs an access check on the file to ensure it can be accessed in the current context.
		/// </summary>
		/// <param name="fileInfo">The file information.</param>
		/// <param name="blob">The BLOB.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> that returns <see langword="true" /> if the file passes the check; otherwise <see langword="false" />.</returns>
		protected virtual Task<bool> CheckFileAccessAsync(UmbrellaAzureBlobStorageFileInfo fileInfo, BlobClient blob, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(true);
		}
		#endregion

		#region IDisposable Support
		private bool _isDisposed = false; // To detect redundant calls

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
					_containerCacheLock.Dispose();

				_isDisposed = true;
			}
		}

		/// <inheritdoc />
		public void Dispose() => Dispose(true); //Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		#endregion
	}
}