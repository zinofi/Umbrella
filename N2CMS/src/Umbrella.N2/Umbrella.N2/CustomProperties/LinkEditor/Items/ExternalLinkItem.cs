using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Umbrella.N2.CustomProperties.LinkEditor.Items
{
    public sealed class ExternalLinkItem : LinkItem
    {
        public override string LinkTypeString
        {
            get { return "External"; }
        }

        public override string Url { get; set; }

        public override void RebaseLinkItem(string fromAppPath, string toAppPath)
        {
            //Do nothing - we don't need to worry about these
        }
    }
}