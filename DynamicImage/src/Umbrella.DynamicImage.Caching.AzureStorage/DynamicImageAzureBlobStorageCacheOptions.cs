using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DynamicImage.Caching.AzureStorage
{
    public class DynamicImageAzureBlobStorageCacheOptions
    {
        /// <summary>
        /// The name of the container in which blobs are stored. Defaults to "dynamicimagecache".
        /// </summary>
        public string ContainerName { get; set; } = "dynamicimagecache";
    }
}