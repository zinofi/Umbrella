using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;

namespace Umbrella.N2.Extensions
{
    public static class IEngineExtensions
    {
        public static bool IsEditPageUrl(this IEngine engine)
        {
            return HttpContext.Current.Request.Url.AbsolutePath.ToLower() != global::N2.Context.Current.ManagementPaths.ResolveResourceUrl("Content/Edit.aspx").ToLower();
        }
    }
}