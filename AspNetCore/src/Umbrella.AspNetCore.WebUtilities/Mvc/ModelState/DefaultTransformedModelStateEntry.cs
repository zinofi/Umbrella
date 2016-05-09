using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelState
{
    public abstract class DefaultTransformedModelState<T>
        where T : DefaultTransformedModelStateEntry
    {
        public List<T> Entries { get; } = new List<T>();
    }

    public abstract class DefaultTransformedModelStateEntry
    {
        public string Key { get; set; }
        public List<string> Errors { get; set; }
    }
}