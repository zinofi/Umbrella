using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.ModelState
{
    public class DefaultTransformedModelStateEntry
    {
        public string Key { get; set; }
        public List<string> Errors { get; set; }
    }
}