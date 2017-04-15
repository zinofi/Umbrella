using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.WebApi;
using Umbrella.N2.CustomProperties.LinkEditor.Items;
using Umbrella.Utilities.Extensions;

namespace Umbrella.N2.CustomProperties.ImageGallery.WebApi
{
    [Authorize]
    public class ImageGalleryUploadFileController : UmbrellaApiController
    {
        #region Private Members
        private readonly IDynamicImageUrlGenerator m_DynamicImageUrlGenerator;
        #endregion
        
        #region Constructors
        public ImageGalleryUploadFileController(ILogger<ImageGalleryUploadFileController> logger,
            IDynamicImageUrlGenerator dynamicImageUrlGenerator)
            : base(logger)
        {
            m_DynamicImageUrlGenerator = dynamicImageUrlGenerator;
        }
        #endregion

        #region Action Methods
        public ImageGalleryItemEditDTO Get(string path)
        {
            try
            {
                var thumbnailOptions = new DynamicImageOptions(path, 150, 150, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg);
                var previewOptions = new DynamicImageOptions(path, 400, 400, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg);

                return new ImageGalleryItemEditDTO
                {
                    Url = path,
                    ThumbnailUrl = m_DynamicImageUrlGenerator.GenerateUrl("dynamicimage", thumbnailOptions, true),
                    PreviewUrl = m_DynamicImageUrlGenerator.GenerateUrl("dynamicimage", previewOptions, true)
                };
            }
            catch(Exception exc) when (Log.WriteError(exc, new { path }))
            {
                throw;
            }
        }
        #endregion
    }
}
