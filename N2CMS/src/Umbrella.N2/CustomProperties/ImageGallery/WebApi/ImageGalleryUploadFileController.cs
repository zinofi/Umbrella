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
        private readonly IDynamicImageUtility m_DynamicImageUtility;
        #endregion
        
        #region Constructors
        public ImageGalleryUploadFileController(ILogger<ImageGalleryUploadFileController> logger,
            IDynamicImageUtility dynamicImageUtility)
            : base(logger)
        {
            m_DynamicImageUtility = dynamicImageUtility;
        }
        #endregion

        #region Action Methods
        public ImageGalleryItemEditDTO Get(string path)
        {
            try
            {
                return new ImageGalleryItemEditDTO
                {
                    Url = path,
                    ThumbnailUrl = m_DynamicImageUtility.GetResizedUrl(path, 150, 150, DynamicResizeMode.UniformFill, toAbsolutePath: true),
                    PreviewUrl = m_DynamicImageUtility.GetResizedUrl(path, 400, 400, DynamicResizeMode.UniformFill, toAbsolutePath: true)
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
