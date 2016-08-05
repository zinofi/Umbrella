using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the provided app-relative path into an absolute Url containing the 
        /// full host name
        /// </summary>
        /// <param name="relativeUrl">App-Relative path</param>
        /// <returns>Provided relativeUrl parameter as fully qualified Url</returns>
        /// <example>~/path/to/foo to http://www.web.com/path/to/foo</example>
        [Obsolete("TODO: Not yet migrated this method!!", true)]
        public static string ToAbsoluteUrl(this string relativeUrl, HttpRequest request)
        {
            //if (string.IsNullOrEmpty(relativeUrl))
            //    return relativeUrl;

            //if (request == null)
            //    return relativeUrl;

            //if (relativeUrl.StartsWith("/"))
            //    relativeUrl = relativeUrl.Insert(0, "~");
            //if (!relativeUrl.StartsWith("~/"))
            //    relativeUrl = relativeUrl.Insert(0, "~/");

            //var url = request.Path;

            //return string.Format("{0}://{1}{2}{3}",
            //    request.Scheme, request.Host.ToUriComponent(), ""); //TODO: VirtualPathUtility.ToAbsolute(relativeUrl));

            throw new NotImplementedException();
        }
    }
}
