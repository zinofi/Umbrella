using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace System.Web.Mvc
{
    public static class EmbeddedResourceHelpers
    {
        private static readonly Page s_Page = new Page();

        public static string GetWebResourceUrl(this UrlHelper helper, Type type, string resourceId)
        {
            if (type == null)
                type = typeof(EmbeddedResourceHelpers);

            return s_Page.ClientScript.GetWebResourceUrl(type, resourceId);
        }
    }
}
