using Umbrella.WebUtilities.DynamicImage.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.DynamicImage.Interfaces
{
    public interface IDynamicImageResizer
    {
        DynamicImage GenerateImage(string virtualPath, int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat imageFormat = DynamicImageFormat.Jpeg);
    }
}
