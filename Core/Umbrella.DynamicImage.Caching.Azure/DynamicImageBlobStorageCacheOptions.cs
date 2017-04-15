using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DynamicImage.Caching.Azure
{
    public class DynamicImageBlobStorageCacheOptions
    {
        /// <summary>
        /// The connection string for the Azure storage account in which the blobs will be stored.
        /// </summary>
        public string StorageConnectionString { get; set; }
        /// <summary>
        /// The name of the container in which blobs are stored. Defaults to "dynamicimagecache".
        /// </summary>
        public string ContainerName { get; set; } = "dynamicimagecache";
    }
}