using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Umbrella.N2.CustomProperties.LinkEditor.Items
{
    public abstract class LinkItemBase
    {
        [JsonIgnore]
        public int SortOrder { get; set; }
        public abstract string LinkTypeString { get; }

        public abstract HtmlString ToHtmlString(string cssClass = "");
        public abstract void RebaseLinkItem(string fromAppPath, string toAppPath);
    }
}