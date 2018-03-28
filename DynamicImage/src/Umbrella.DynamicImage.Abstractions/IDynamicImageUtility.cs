using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageUtility
    {
        DynamicImageFormat ParseImageFormat(string format);
        (DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) TryParseUrl(string dynamicImagePathPrefix, string relativeUrl);
        bool ImageOptionsValid(DynamicImageOptions imageOptions, DynamicImageConfigurationOptions configOptions);
        string GenerateVirtualPath(string dynamicImagePathPrefix, DynamicImageOptions options);
    }
}