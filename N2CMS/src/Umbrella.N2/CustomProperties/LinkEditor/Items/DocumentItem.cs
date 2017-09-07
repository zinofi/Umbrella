using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Umbrella.N2.CustomProperties.LinkEditor.Items
{
    public sealed class DocumentItem : LinkItem
    {
        public override string LinkTypeString
        {
            get { return "Document"; }
        }

        public override string Url { get; set; }

        public override void RebaseLinkItem(string fromAppPath, string toAppPath)
        {
            Url = global::N2.Web.Url.Rebase(Url, fromAppPath, toAppPath);
        }
    }
}