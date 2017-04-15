using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageUrlGenerator
    {
        string GenerateUrl(string dynamicImagePathPrefix, DynamicImageOptions options, bool toAbsolutePath = false);
    }
}