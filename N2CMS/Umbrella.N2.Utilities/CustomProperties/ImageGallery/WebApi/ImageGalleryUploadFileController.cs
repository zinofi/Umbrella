using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbrella.Legacy.WebUtilities.WebApi;
using Umbrella.N2.CustomProperties.LinkEditor.Items;
using Umbrella.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage.Enumerations;

namespace Umbrella.N2.CustomProperties.ImageGallery.WebApi
{
    [Authorize]
    public class ImageGalleryUploadFileController : UmbrellaApiController
    {
        public ImageGalleryItemEditDTO Get(string path)
        {
            return new ImageGalleryItemEditDTO
            {
                Url = path,
                ThumbnailUrl = DynamicImageUtility.GetResizedUrl(path, 150, 150, DynamicResizeMode.UniformFill, toAbsolutePath: true),
                PreviewUrl = DynamicImageUtility.GetResizedUrl(path, 400, 400, DynamicResizeMode.UniformFill, toAbsolutePath: true)
            };
        }
    }
}
