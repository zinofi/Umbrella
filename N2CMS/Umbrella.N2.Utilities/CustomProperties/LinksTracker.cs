using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Edit.LinkTracker;
using N2.Plugin;
using N2.Persistence.Finder;
using N2.Persistence;
using N2.Web;
using Umbrella.N2.CustomProperties.LinkEditor;
using Umbrella.N2.CustomProperties.LinkEditor.Items;
using N2.Details;
using N2.Configuration;

namespace Umbrella.N2.CustomProperties
{
    [Service(Replaces = typeof(Tracker))]
    public class LinksTracker : Tracker
    {
        public LinksTracker(IPersister persister, IUrlParser urlParser, ConnectionMonitor connections, IErrorNotifier errorHandler, EditSection config)
            : base(persister, urlParser, connections, errorHandler, config)
        {
        }

        public override IList<ContentDetail> FindLinks(string html)
        {
            //Looks like when a page is SAVED, this gets called - database records are deleted to track links
            //These records are scanned when a page is being deleted to detect conflicts
            //Obviously a more effecient way of doing things as opposed to scanning the whole site for dependencies at once!
            List<ContentDetail> links = base.FindLinks(html).ToList();
            if (links.Count == 0)
            {
                //No links found - might be dealing with a LinkItemCollection in which case we have a JSON string
                LinkItemCollection linkCollection;

                if (LinkItemCollection.TryParse(html, out linkCollection))
                {
                    links = links.Concat(linkCollection.OfType<LinkItem>().Select(x => ContentDetail.Multi(LinkDetailName, false, 0, null, null, x.Url))).ToList();
                }
            }

            return links;
        }
    }
}