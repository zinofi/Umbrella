using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Helpers
{
    public static class HtmlHeadHelpers
    {
        public static MvcHtmlString MetaTag(this HtmlHelper helper, string name, string content)
        {
            if(!string.IsNullOrWhiteSpace(content))
            {
                TagBuilder tb = new TagBuilder("meta");
                tb.MergeAttribute("name", name);
                tb.MergeAttribute("content", content);

                return new MvcHtmlString(tb.ToString(TagRenderMode.SelfClosing));
            }

            return null;
        }
    }
}
