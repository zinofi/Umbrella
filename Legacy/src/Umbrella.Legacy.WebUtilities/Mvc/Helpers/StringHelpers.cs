using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Helpers
{
    public static class StringHelpers
    {
        public static IHtmlString Nl2Br(this HtmlHelper helper, string value)
            => helper.Raw(value.Replace("\n", "<br />"));
    }
}