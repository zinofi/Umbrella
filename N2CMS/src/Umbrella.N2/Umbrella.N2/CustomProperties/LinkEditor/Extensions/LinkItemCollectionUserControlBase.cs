using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2;

namespace Umbrella.N2.CustomProperties.LinkEditor.Extensions
{
    public abstract class LinkItemCollectionUserControlBase : System.Web.UI.UserControl
    {
        public ContentItem CurrentItem { get; internal set; }
    }
}