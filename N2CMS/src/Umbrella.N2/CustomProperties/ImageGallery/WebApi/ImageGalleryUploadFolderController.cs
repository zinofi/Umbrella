using Microsoft.Extensions.Logging;
using N2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.WebApi;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.N2.CustomProperties.ImageGallery.WebApi
{
    [Authorize]
    public class ImageGalleryUploadFolderController : UmbrellaApiController
    {
        #region Private Members
        private readonly IDynamicImageUtility m_DynamicImageUtility;
        private readonly IUmbrellaWebHostingEnvironment m_UmbrellaHostingEnvironment;
        #endregion

        #region Constructors
        public ImageGalleryUploadFolderController(ILogger<ImageGalleryUploadFolderController> logger,
            IDynamicImageUtility dynamicImageUtility,
            IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment)
            : base(logger)
        {
            m_DynamicImageUtility = dynamicImageUtility;
            m_UmbrellaHostingEnvironment = umbrellaHostingEnvironment;
        }
        #endregion

        #region Action Methods
        public IHttpActionResult Get(string folderPath)
        {
            try
            {
                if (folderPath.StartsWith("/"))
                {
                    IList<string> uploadFolders = Context.Current.EditManager.UploadFolders;

                    string firstSegment = folderPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    if (!string.IsNullOrEmpty(firstSegment) && uploadFolders.Any(x => x.ToLower() == "~/" + firstSegment.ToLower() + "/"))
                    {
                        string folderPathLowered = folderPath.ToLower();

                        IEnumerable<string> files = Directory.EnumerateFiles(HostingEnvironment.MapPath("~" + folderPath)).Select(x => folderPathLowered + x.Replace("\\", "/").ToLower().Split(new[] { folderPathLowered }, StringSplitOptions.RemoveEmptyEntries)[1]);

                        //Ensure the windows Thumbs.db file is excluded
                        return Ok(files.Where(x => Path.GetExtension(x).ToLower() != ".db").Select(path => new ImageGalleryItemEditDTO
                        {
                            Url = path,
                            ThumbnailUrl = m_UmbrellaHostingEnvironment.MapWebPath(m_DynamicImageUtility.GenerateVirtualPath("dynamicimage", new DynamicImageOptions(path, 150, 150, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg))),
                            PreviewUrl = m_UmbrellaHostingEnvironment.MapWebPath(m_DynamicImageUtility.GenerateVirtualPath("dynamicimage", new DynamicImageOptions(path, 400, 400, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg)))
                        }));
                    }
                }

                return NotFound();
            }
            catch(Exception exc) when (Log.WriteError(exc, new { folderPath }))
            {
                throw;
            }
        }
        #endregion
    }
}