using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.ModelState
{
    public abstract class DefaultTransformedModelState<T>
        where T : DefaultTransformedModelStateEntry
    {
        public List<T> Entries { get; } = new List<T>();
    }
}
