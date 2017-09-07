using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbrella.N2.CustomProperties.LinkEditor.Items;

namespace Umbrella.N2.CustomProperties.ImageGallery
{
    public class ImageGalleryItemEditDTO
    {
        public string Url { get; set; }
        public string AltText { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PreviewUrl { get; set; }
    }
}