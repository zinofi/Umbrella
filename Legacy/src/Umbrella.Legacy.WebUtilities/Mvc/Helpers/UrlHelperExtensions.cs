using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Extensions;

namespace Umbrella.Legacy.WebUtilities.Mvc.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string ContentLower(this UrlHelper helper, string contentPath)
            => helper.Content(contentPath)?.ToLowerInvariant();

        public static string ContentAbsolute(this UrlHelper helper, string contentPath, string schemeOverride = null, string hostOverride = null, int portOverride = 0)
            => contentPath.ToAbsoluteUrl(helper.RequestContext.HttpContext.Request.Url, schemeOverride, hostOverride, portOverride);

        public static string ContentAbsoluteLower(this UrlHelper helper, string contentPath, string schemeOverride = null, string hostOverride = null, int portOverride = 0)
            => ContentAbsolute(helper, contentPath, schemeOverride, hostOverride, portOverride)?.ToLowerInvariant();
    }
}