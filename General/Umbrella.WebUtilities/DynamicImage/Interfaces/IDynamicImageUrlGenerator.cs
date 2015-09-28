using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.DynamicImage.Interfaces
{
    public interface IDynamicImageUrlGenerator
    {
        string GenerateUrl(DynamicImageOptions options, bool toAbsolutePath = false);
    }
}