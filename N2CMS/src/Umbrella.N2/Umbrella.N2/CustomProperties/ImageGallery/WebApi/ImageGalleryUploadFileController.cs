using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.WebApi;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.N2.CustomProperties.ImageGallery.WebApi
{
    [Authorize]
    public class ImageGalleryUploadFileController : UmbrellaApiController
    {
        #region Private Members
        private readonly IDynamicImageUtility m_DynamicImageUtility;
        private readonly IUmbrellaWebHostingEnvironment m_UmbrellaHostingEnvironment;
        #endregion
        
        #region Constructors
        public ImageGalleryUploadFileController(ILogger<ImageGalleryUploadFileController> logger,
            IDynamicImageUtility dynamicImageUtility,
            IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment)
            : base(logger)
        {
            m_DynamicImageUtility = dynamicImageUtility;
            m_UmbrellaHostingEnvironment = umbrellaHostingEnvironment;
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
                    ThumbnailUrl = m_UmbrellaHostingEnvironment.MapWebPath(m_DynamicImageUtility.GenerateVirtualPath("dynamicimage", thumbnailOptions)),
                    PreviewUrl = m_UmbrellaHostingEnvironment.MapWebPath(m_DynamicImageUtility.GenerateVirtualPath("dynamicimage", previewOptions))
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