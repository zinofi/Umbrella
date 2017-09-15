using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.FileSystem.AzureStorage
{
    public class UmbrellaAzureBlobStorageFileProviderOptions
    {
        /// <summary>
        /// The connection string for the Azure storage account in which the blobs will be stored.
        /// </summary>
        public string StorageConnectionString { get; set; }
    }
}