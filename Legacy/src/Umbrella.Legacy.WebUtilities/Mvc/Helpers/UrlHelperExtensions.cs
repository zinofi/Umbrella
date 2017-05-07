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
        public static string ContentAbsolute(this UrlHelper helper, string contentPath)
            => contentPath.ToAbsoluteUrl();
    }
}