using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using Newtonsoft.Json;

namespace Umbrella.N2.CustomProperties.LinkEditor.Items
{
    public sealed class InternalLinkItem : LinkItem
    {
        public override string LinkTypeString
        {
            get { return "Internal"; }
        }

        public int PageId { get; set; }

        [JsonIgnore]
        public ContentItem Page
        {
            get { return Find.Query().Where(x => x.ID == PageId && x.State == ContentState.Published).FirstOrDefault(); }
        }

        public override string Url
        {
            get
            {
                //With caching enabled, this should be a low resource consuming task
                ContentItem item = Find.Query().Where(x => x.ID == PageId && x.State == ContentState.Published).FirstOrDefault();
                if (item != null)
                    return item.Url;

                return null;
            }
            set { /*do nothing - we aren't interested in the link as it might change - we can lookup using the page id*/ }
        }

        public override void RebaseLinkItem(string fromAppPath, string toAppPath)
        {
            //Do nothing - we dont care about Internal links
        }
    }
}