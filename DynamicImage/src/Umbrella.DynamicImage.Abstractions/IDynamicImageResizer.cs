using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageResizer
    {
        DynamicImageItem GenerateImage(string virtualPath, int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat imageFormat = DynamicImageFormat.Jpeg);
    }
}
