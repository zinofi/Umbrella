using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Optimization;

namespace Umbrella.Legacy.WebUtilities.Bundling
{
    public class CssRewriteUrlVirtualPathTransform : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
            => new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath).ToLowerInvariant(), input);
    }
}