using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage.Enumerations;

namespace Umbrella.WebUtilities.DynamicImage
{
    public struct DynamicImageOptions
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public DynamicResizeMode Mode { get; set; }
        public DynamicImageFormat Format { get; set; }
        public string OriginalVirtualPath { get; set; }
    }
}
