using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.DynamicImage.Interfaces
{
    public interface IDynamicImageUtilityResolver
    {
        IDynamicImageUtility GetInstance();
    }
}
