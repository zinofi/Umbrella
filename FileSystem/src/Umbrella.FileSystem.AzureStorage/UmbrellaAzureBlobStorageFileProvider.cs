using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.AzureStorage
{
	// TODO V3: Design - Create a generic overload of this class that allows the options type to be specified.
	// That way a consuming application could extend this provider if required with custom options
	// so that multiple file providers could be added to the same application concurrently and registered with DI
	// without conflicting with each other.
	public class UmbrellaAzureBlobStorageFileProvider : UmbrellaFileProvider<UmbrellaAzureBlobStorageFileInfo, UmbrellaAzureBlobStorageFileProviderOptions>, IUmbrellaAzureBlobStorageFileProvider
	{
		#region Private Static Members
		private static readonly char[] s_DirectorySeparatorArray = new[] { '/', '\\' };
		#endregion

		#region Protected Properties
		protected CloudStorageAccount StorageAccount { get; }
		protected CloudBlobClient BlobClient { get; }
		protected ConcurrentDictionary<string, bool> ContainerResolutionCache { get; }
		#endregion

		#region Constructors
		public UmbrellaAzureBlobStorageFileProvider(ILoggerFactory loggerFactory,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter,
			UmbrellaAzureBlobStorageFileProviderOptions options)
			: base(loggerFactory.CreateLogger<UmbrellaAzureBlobStorageFileProvider>(), loggerFactory, mimeTypeUtility, genericTypeConverter, options)
		{
			Guard.ArgumentNotNullOrWhiteSpace(options.StorageConnectionString, nameof(options.StorageConnectionString));

			StorageAccount = CloudStorageAccount.Parse(options.StorageConnectionString);
			BlobClient = StorageAccount.CreateCloudBlobClient();

			if (Options.CacheContainerResolutions)
				ContainerResolutionCache = new ConcurrentDictionary<string, bool>();
		}
		#endregion

		#region IUmbrellaAzureBlobStorageFileProvider Members
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

		#region Overridden Methods
		protected override async Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			//TODO: Need to keep a list of incoming container names and map these to cleaned names so that we can throw an
			//exception in cases where two different incoming names map to the same cleaned name which could potentially
			//cause issues with unintentionally overwriting files that have the same name

			//TODO: Use a regex here to only allow the characters permitted by Azure which are:
			//Length: between 3 and 63 chars
			//Lowercase letters
			//Numbers
			//Hyphens
			//Can't contain consecutive hyphens
			//Begin and end with a letter or number
			StringBuilder pathBuilder = new StringBuilder(subpath)
				.Trim(' ', '\\', '/', '~', '_', ' ')
				.Replace("_", "")
				.Replace(" ", "");

			string cleanedPath = pathBuilder.ToString();

			if (Log.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
				Log.WriteDebug(new { subpath, cleanedPath });

			string[] parts = cleanedPath.Split(s_DirectorySeparatorArray, StringSplitOptions.RemoveEmptyEntries);

			//TODO: Should extend this to allow for nested folder structures
			if (parts.Length != 2)
				throw new UmbrellaFileSystemException($"The value for {nameof(subpath)} must contain exactly 2 segments, i.e. folder name and file name. The folder name is used as the blob storage container name. The invalid value is {subpath}");

			string containerName = parts[0].ToLowerInvariant();
			string fileName = parts[1];

			CloudBlobContainer container = BlobClient.GetContainerReference(containerName);

			// TODO: Replace this mechanism with the MultiCache when it supports AsyncLazy internally. That way the calls to AzureStorage will only happen once. Possibility for a race condition
			// with the current approach. Hardly a big deal though!!
			if (ContainerResolutionCache != null && !ContainerResolutionCache.ContainsKey(containerName))
			{
#if NET461
				await container.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);
#else
				await container.CreateIfNotExistsAsync().ConfigureAwait(false);
#endif
				// The value can be anything here but we need to use ConcurrentDictioary because there isn't a ConcurrentHashSet type.
				// Could implement our own locking mechanism around a HashSet but not worth it. Maybe consider in the future or see TODO above.
				ContainerResolutionCache.TryAdd(containerName, true);
			}

			CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

			//The call to ExistsAsync should force the properties of the blob to be populated
#if NET461
            if (!isNew && !await blob.ExistsAsync(cancellationToken).ConfigureAwait(false))
                return null;
#else
			if (!isNew && !await blob.ExistsAsync().ConfigureAwait(false))
				return null;
#endif

			if (!await CheckFileAccessAsync(containerName, fileName, cancellationToken))
				throw new UmbrellaFileAccessDeniedException(subpath);

			return new UmbrellaAzureBlobStorageFileInfo(FileInfoLoggerInstance, MimeTypeUtility, GenericTypeConverter, subpath, this, blob, isNew);
		}
		#endregion

		#region Protected Methods
		protected virtual Task<bool> CheckFileAccessAsync(string containerName, string fileName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(true);
		}
		#endregion
	}
}