using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.FileSystem.AzureStorage
{
	/// <summary>
	/// The options for the UmbrellaAzureBlobStorageFileProvider.
	/// </summary>
	public class UmbrellaAzureBlobStorageFileProviderOptions : IUmbrellaFileProviderOptions, ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		/// <summary>
		/// The connection string for the Azure storage account in which the blobs will be stored.
		/// </summary>
		public string StorageConnectionString { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to cache the result of calls which check if a Blob container exists.
		/// This is to improve efficiency by preventing repeated redundant calls out to the Azure Storage service when in reality
		/// we only need to make this call once per container.
		/// </summary>
		public bool CacheContainerResolutions { get; set; } = true;

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
		public void Sanitize() => StorageConnectionString = StorageConnectionString?.Trim();

		/// <summary>
		/// Validates this instance.
		/// </summary>
		public void Validate() => Guard.ArgumentNotNullOrWhiteSpace(StorageConnectionString, nameof(StorageConnectionString));
	}
}