using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public static class StringHelpers
    {
        public static IHtmlString Nl2Br(this HtmlHelper helper, string value)
            => helper.Raw(value.Replace("\n", "<br />"));
    }
}