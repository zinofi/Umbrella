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
using Umbrella.Utilities.Extensions;

namespace Umbrella.N2.CustomProperties.ImageGallery.WebApi
{
    [Authorize]
    public class ImageGalleryUploadFolderController : UmbrellaApiController
    {
        #region Private Members
        private readonly IDynamicImageUrlGenerator m_DynamicImageUrlGenerator;
        #endregion

        #region Constructors
        public ImageGalleryUploadFolderController(ILogger<ImageGalleryUploadFolderController> logger,
            IDynamicImageUrlGenerator dynamicImageUrlGenerator)
            : base(logger)
        {
            m_DynamicImageUrlGenerator = dynamicImageUrlGenerator;
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
                            ThumbnailUrl = m_DynamicImageUrlGenerator.GenerateUrl("dynamicimage", new DynamicImageOptions(path, 150, 150, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg), true),
                            PreviewUrl = m_DynamicImageUrlGenerator.GenerateUrl("dynamicimage", new DynamicImageOptions(path, 400, 400, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg), true)
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