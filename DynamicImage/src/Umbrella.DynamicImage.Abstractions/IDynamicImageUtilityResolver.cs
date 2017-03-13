using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
    public interface IDynamicImageUtilityResolver
    {
        IDynamicImageUtility GetInstance();
    }
}
