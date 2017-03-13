using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageUtility
    {
        DynamicImageItem GetImage(int width, int height, DynamicResizeMode mode, string originalExtension, string path);
        string GetResizedUrl(string path, int width, int height, DynamicResizeMode mode, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false);
        DynamicImageFormat ParseImageFormat(string format);
    }
}