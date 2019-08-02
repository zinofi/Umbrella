using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.FileSystem.AzureStorage
{
	/// <summary>
	/// The options for the UmbrellaAzureBlobStorageFileProvider.
	/// </summary>
	public class UmbrellaAzureBlobStorageFileProviderOptions
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
    }
}