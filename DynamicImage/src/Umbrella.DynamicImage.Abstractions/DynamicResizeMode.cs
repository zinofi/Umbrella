using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
    public enum DynamicResizeMode
    {
        UseWidth = 0,
        UseHeight = 1,
        Fill = 2,
        Uniform = 3,
        UniformFill = 4
    }
}
