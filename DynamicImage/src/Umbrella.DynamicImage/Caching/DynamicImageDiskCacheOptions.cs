using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DynamicImage.Caching
{
    public class DynamicImageDiskCacheOptions
    {
        /// <summary>
        /// The physical folder path under which images will be cached.
        /// </summary>
        public string PhysicalFolderPath { get; set; }
    }
}