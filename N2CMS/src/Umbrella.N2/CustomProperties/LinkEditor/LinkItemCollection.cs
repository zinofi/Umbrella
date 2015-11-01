using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using N2.Web.UI;
using System.Linq.Expressions;
using N2;
using System.Threading.Tasks;
using N2.Plugin;
using System.Reflection;
using Umbrella.N2.CustomProperties.LinkEditor.Items;
using Umbrella.N2.CustomProperties.LinkEditor.Extensions;
using System.Threading;
using Newtonsoft.Json;

namespace Umbrella.N2.CustomProperties.LinkEditor
{
    public class LinkItemCollection : List<LinkItemBase>
    {
        #region Public Static Members
        public static readonly LinkItemCollection Empty = new LinkItemCollection();
        #endregion

        #region Private Static Members
        private static Dictionary<int, Dictionary<string, LinkItemCollection>> s_Dictionary = new Dictionary<int, Dictionary<string, LinkItemCollection>>();
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        #endregion

        #region Internal Static Properties
        internal static List<ILinkItemCollectionPlugin> PluginList { get; set; }
        #endregion

        #region Public Properties
        public IEnumerable<InternalLinkItem> Pages
        {
            get { return this.OfType<InternalLinkItem>(); }
        }

        public IEnumerable<ExternalLinkItem> ExternalLinks
        {
            get { return this.OfType<ExternalLinkItem>(); }
        }

        public IEnumerable<LinkItem> AllLinks
        {
            get { return this.OfType<LinkItem>(); }
        }

        public IEnumerable<ImageItem> Images
        {
            get { return this.OfType<ImageItem>(); }
        }

        public IEnumerable<DocumentItem> Documents
        {
            get { return this.OfType<DocumentItem>(); }
        }
        #endregion

        #region N2 Persister Event Handlers
        private static void Persister_ItemDeleted(object sender, ItemEventArgs e)
        {
            if (s_Dictionary.ContainsKey(e.AffectedItem.ID))
            {
                lock (s_Lock)
                {
                    if (s_Dictionary.ContainsKey(e.AffectedItem.ID))
                    {
                        s_Dictionary.Remove(e.AffectedItem.ID);
                    }
                }
            }
        }

        private static void Persister_ItemSaved(object sender, ItemEventArgs e)
        {
            //TODO: We only want to clear the cache for published pages - not page versions!
            if (s_Dictionary.ContainsKey(e.AffectedItem.ID))
            {
                lock (s_Lock)
                {
                    if (s_Dictionary.ContainsKey(e.AffectedItem.ID))
                    {
                        s_Dictionary.Remove(e.AffectedItem.ID);
                    }
                }
            }
        }
        #endregion

        #region Public Static Methods
        public static bool TryParse(string json, out LinkItemCollection links)
        {
            try
            {
                links = Parse(json);
                return true;
            }
            catch (Exception)
            {
                links = Empty;
                return false;
            }
        }
        #endregion

        #region Internal Static Methods
        public static LinkItemCollection FindByPageAndPropertyName(ContentItem item, string propertyName)
        {
            //For some reason, the N2 events will not attach when this is done in the static constructor
            //This method is ultimately the entry point for populating the cache, so we can attach the event handlers
            //in here the first time this method is executed
            if (!s_Initialized)
            {
                lock (s_Lock)
                {
                    if (!s_Initialized)
                    {
                        Context.Persister.ItemSaved += Persister_ItemSaved;
                        Context.Persister.ItemDeleted += Persister_ItemDeleted;

                        s_Initialized = true;
                    }
                }
            }

            EnsureCache(item, propertyName);

            //TODO: This caching mechanism could present a problem with dynamic properties... maybe
            return s_Dictionary[item.ID].ContainsKey(propertyName)
                ? s_Dictionary[item.ID][propertyName]
                : Empty;
        }
        #endregion

        #region Internal Instance Methods
        public string ToJSONString() => JsonConvert.SerializeObject(ToArray());
        #endregion

        #region Private Static Methods
        private static LinkItemCollection Parse(string json)
        {
            LinkItemCollection coll = new LinkItemCollection();

            Dictionary<string, object>[] jsonItems = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(json);

            LinkItemBase[] arrLinkItemBase = new LinkItemBase[jsonItems.Length];

            //Execute this in parallel if possible to save time with json serialization/deserization for child items
            Parallel.ForEach(jsonItems, (x, state, index) =>
            {
                Dictionary<string, object> dic = (Dictionary<string, object>)x;

                string linkType = dic["LinkTypeString"].ToString();

                //HACK: Find a better way of doing this - we dont want to have to serialize/deserialize this again!
                //Need a good way of initially getting the array elements
                string json2 = JsonConvert.SerializeObject(dic);

                LinkItemBase itemBase = null;

                if (linkType == "Image")
                {
                    itemBase = JsonConvert.DeserializeObject<ImageItem>(json2);
                }
                else if (linkType == "Internal")
                {
                    itemBase = JsonConvert.DeserializeObject<InternalLinkItem>(json2);
                }
                else if (linkType == "External")
                {
                    itemBase = JsonConvert.DeserializeObject<ExternalLinkItem>(json2);
                }
                else if (linkType == "Document")
                {
                    itemBase = JsonConvert.DeserializeObject<DocumentItem>(json2);
                }
                else
                {
                    if (PluginList != null)
                    {
                        ILinkItemCollectionPlugin plugin = PluginList.Find(y => y.Name == linkType);
                        if (plugin != null)
                        {
                            itemBase = JsonConvert.DeserializeObject(json2, plugin.PluginType) as LinkItemBase;
                            if (itemBase == null)
                                throw new ApplicationException("The following json string for linkType " + linkType + " could not be parsed: " + json2);
                        }
                    }
                }

                itemBase.SortOrder = (int)index;

                arrLinkItemBase[index] = itemBase;
            });

            //Add the array items to the LinkItemCollection
            coll.AddRange(arrLinkItemBase);

            return coll;
        }

        private static void EnsureCache(ContentItem item, string propertyName)
        {
            if (!s_Dictionary.ContainsKey(item.ID))
            {
                lock (s_Lock)
                {
                    if (!s_Dictionary.ContainsKey(item.ID))
                    {
                        s_Dictionary.Add(item.ID, new Dictionary<string, LinkItemCollection>());
                    }
                }
            }

            if (!s_Dictionary[item.ID].ContainsKey(propertyName))
            {
                lock (s_Lock)
                {
                    if (!s_Dictionary[item.ID].ContainsKey(propertyName))
                    {
                        string value = item.GetDetail<string>(propertyName, string.Empty);
                        if (!string.IsNullOrEmpty(value))
                        {
                            LinkItemCollection coll = Parse(value);
                            s_Dictionary[item.ID].Add(propertyName, coll);
                        }
                    }
                }
            }
        }
        #endregion
    }
}