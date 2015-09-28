using AutoMapper;
using N2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Umbrella.Legacy.WebUtilities.WebApi;
using Umbrella.N2.CustomProperties.LinkEditor.Items;
using Umbrella.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage.Enumerations;

namespace Umbrella.N2.CustomProperties.ImageGallery.WebApi
{
    [Authorize]
    public class ImageGalleryUploadFolderController : UmbrellaApiController
    {
        public IHttpActionResult Get(string folderPath)
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
                    return Ok(files.Where(x => Path.GetExtension(x).ToLower() != ".db").Select(x => new ImageGalleryItemEditDTO
                    {
                        Url = x,
                        ThumbnailUrl = DynamicImageUtility.GetResizedUrl(x, 150, 150, DynamicResizeMode.UniformFill, toAbsolutePath: true),
                        PreviewUrl = DynamicImageUtility.GetResizedUrl(x, 400, 400, DynamicResizeMode.UniformFill, toAbsolutePath: true)
                    }));
                }
            }

            return NotFound();
        }
    }
}
