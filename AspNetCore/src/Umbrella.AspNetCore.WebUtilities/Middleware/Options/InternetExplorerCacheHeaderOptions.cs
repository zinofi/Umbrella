using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.AspNetCore.WebUtilities.Middleware.Options
{
    public class InternetExplorerCacheHeaderOptions
    {
        public List<string> UserAgentKeywords { get; set; } = new List<string> { "MSIE", "Trident" };
        public List<string> Methods { get; set; } = new List<string> { "GET", "HEAD" };
        public List<string> ContentTypes { get; set; } = new List<string>();
    }
}