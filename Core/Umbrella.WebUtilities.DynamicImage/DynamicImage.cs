using Umbrella.WebUtilities.DynamicImage.Enumerations;
using Umbrella.WebUtilities.DynamicImage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.DynamicImage
{
    public class DynamicImage
    {
        public byte[] Content { get; set; }
        public DateTime LastModified { get; set; }
        public DynamicImageOptions ImageOptions { get; set; }
        public string CachedVirtualPath { get; set; }
    }
}
