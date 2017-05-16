using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Extensions;

namespace System.Web.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string ContentLower(this UrlHelper helper, string contentPath)
            => helper.Content(contentPath)?.ToLowerInvariant();

        public static string ContentAbsolute(this UrlHelper helper, string contentPath)
            => contentPath.ToAbsoluteUrl();

        public static string ContentAbsoluteLower(this UrlHelper helper, string contentPath)
            => ContentAbsolute(helper, contentPath)?.ToLowerInvariant();
    }
}