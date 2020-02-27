using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
	// TODO: What was this ever for?? Can it be removed?
	// Probably here from when the Marshalls Visualizers Project was initially done.
	// Unless the N2 project uses it?
    public interface IDynamicImageUtilityResolver
    {
        IDynamicImageUtility Instance { get; }
    }
}