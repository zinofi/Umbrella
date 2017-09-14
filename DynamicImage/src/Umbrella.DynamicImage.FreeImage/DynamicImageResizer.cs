using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbrella.Utilities.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Extensions;
using Umbrella.DynamicImage.Abstractions;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;
using System.Threading;

namespace Umbrella.DynamicImage.FreeImage
{
    public class DynamicImageResizer : DynamicImageResizerBase
    {
        #region Constructors
        public DynamicImageResizer(ILogger<DynamicImageResizer> logger,
            IDynamicImageCache dynamicImageCache)
            : base(logger, dynamicImageCache)
        {
        }
        #endregion

        #region Overridden Methods
        protected override byte[] ResizeImage(byte[] originalImage, DynamicImageOptions options)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}