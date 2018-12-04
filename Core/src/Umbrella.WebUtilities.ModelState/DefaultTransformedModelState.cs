using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.ModelState
{
    // TODO: Why are these in their own package?? Can't remember why I did this. Must have been a good reason :s
    public class DefaultTransformedModelState<T>
        where T : DefaultTransformedModelStateEntry
    {
        public List<T> Entries { get; } = new List<T>();
    }
}