using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage.Enumerations;

namespace Umbrella.WebUtilities.DynamicImage.Interfaces
{
    public interface IDynamicImageUtility
    {
        DynamicImage GetImage(int width, int height, DynamicResizeMode mode, string originalExtension, string path);
        string GetResizedUrl(string path, int width, int height, DynamicResizeMode mode, DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false);
        DynamicImageFormat ParseImageFormat(string format);
    }
}