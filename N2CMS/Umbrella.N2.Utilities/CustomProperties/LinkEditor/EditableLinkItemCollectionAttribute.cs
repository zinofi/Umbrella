using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using System.Web.UI;
using N2;
using System.Reflection;
using System.Web.Mvc;
using Umbrella.N2.CustomProperties.LinkEditor.Extensions;
using Umbrella.N2.CustomProperties.LinkEditor.Controls;
using Umbrella.N2.CustomProperties.LinkEditor.Items;

namespace Umbrella.N2.CustomProperties.LinkEditor
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EditableLinkItemCollectionAttribute : AbstractEditableAttribute, IRelativityTransformer
    {
        #region Public Properties
        public bool DisableLinksOption { get; set; }
        public bool DisableImageOption { get; set; }
        public Type[] PluginTypesAvailable { get; set; }
        #endregion

        #region Constructors
        public EditableLinkItemCollectionAttribute(string title, int sortOrder, params Type[] pluginTypes)
            : base(title, sortOrder)
        {
            if (pluginTypes != null && pluginTypes.Length > 0)
            {
                Type pluginType = typeof(ILinkItemCollectionPlugin);

                if (!pluginTypes.All(x => pluginType.IsAssignableFrom(x)))
                    throw new ArgumentException("The property " + title + " specifies a parameter for a plugin which does not implement ILinkItemCollectionPlugin");

                PluginTypesAvailable = pluginTypes;
            }
        }

        public EditableLinkItemCollectionAttribute(string title, string name, int sortOrder, params Type[] pluginTypes)
            : this(title, sortOrder, pluginTypes)
        {
            Name = name;
        }
        #endregion

        #region Overridden Methods
        protected override Control AddEditor(Control container)
        {
            LinkEditorControl c = new LinkEditorControl();
            container.Controls.Add(c);
            return c;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            LinkItemCollection coll = LinkItemCollection.FindByPageAndPropertyName(item, Name);

            List<ILinkItemCollectionPlugin> plugins = PluginTypesAvailable != null
                ? LinkItemCollection.PluginList.Where(x => PluginTypesAvailable.Contains(x.GetType())).ToList()
                : new List<ILinkItemCollectionPlugin>();

            ((LinkEditorControl)editor).Initialize(item, coll.ToJSONString(), plugins, coll == null ? 0 : coll.Count, !DisableLinksOption, !DisableImageOption);
        }

        public override bool UpdateItem(ContentItem item, System.Web.UI.Control editor)
        {
            string value = ((LinkEditorControl)editor).Value;

            item.SetDetail(Name, value, typeof(string));

            return true;
        }
        #endregion

        #region IRelativityTransformer Members
        public string Rebase(string value, string fromAppPath, string toAppPath)
        {
            //The value being passed in is the raw JSON value stored for the property
            //We need to identify all links that need to be rebased
            LinkItemCollection coll = LinkItemCollection.Empty;
            if (LinkItemCollection.TryParse(value, out coll))
            {
                foreach (LinkItemBase item in coll)
                    item.RebaseLinkItem(fromAppPath, toAppPath);
            }

            return coll.ToJSONString();
        }

        public RelativityMode RelativeWhen
        {
            get { return RelativityMode.Always; }
        }
        #endregion
    }
}