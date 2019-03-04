using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.Middleware.Options
{
    public class FrontEndCompressionMiddlewareOptions
    {
        public string[] FrontEndRootFolderAppRelativePaths { get; set; }
        public string[] TargetFileExtensions { get; set; } = new string[] { ".css", ".js" };
        public bool CacheEnabled { get; set; } = true;
        public TimeSpan CacheTimeout { get; set; } = TimeSpan.FromHours(1);
        public bool CacheSlidingExpiration { get; set; } = true;
        public bool WatchFiles { get; set; }
        public string AcceptEncodingHeaderKey { get; set; } = "Accept-Encoding";
        public Action<IOwinContext, HashSet<string>> AcceptEncodingModifier { get; set; }
	}
}