using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbrella.Utilities;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
    //TODO: Add unit tests
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the provided app-relative path into an absolute Url containing the 
        /// full host name
        /// </summary>
        /// <param name="relativeUrl">App-Relative path</param>
        /// <returns>Provided relativeUrl parameter as fully qualified Url</returns>
        /// <example>~/path/to/foo to http://www.web.com/path/to/foo</example>
        public static string ToAbsoluteUrl(this string relativeUrl, Uri requestUri, string schemeOverride = null, string hostOverride = null, int portOverride = 0)
        {
            Guard.ArgumentNotNullOrWhiteSpace(relativeUrl, nameof(relativeUrl));
            Guard.ArgumentNotNull(requestUri, nameof(requestUri));
            
            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");

            var port = portOverride > 0 ? portOverride.ToString() : (requestUri.Port != 80 ? (":" + requestUri.Port) : string.Empty);

            return string.Format("{0}://{1}{2}{3}",
                schemeOverride ?? requestUri.Scheme, hostOverride ?? requestUri.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
        }
    }
}
