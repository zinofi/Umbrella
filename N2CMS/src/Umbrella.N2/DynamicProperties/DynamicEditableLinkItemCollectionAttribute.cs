using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbrella.N2.CustomProperties.LinkEditor;
using Umbrella.N2.CustomProperties.LinkEditor.Extensions;
using Umbrella.N2.DynamicProperties.Utilities;

namespace Umbrella.N2.DynamicProperties
{
    //TODO: This is untested - I have a feeling this won't even work because of the way the LinkItemCollection caching works
    public class DynamicEditableLinkItemCollectionAttribute : EditableLinkItemCollectionAttribute
    {
        public DynamicEditableLinkItemCollectionAttribute(string title, int sortOrder, params Type[] pluginTypes)
            : base(title, sortOrder, pluginTypes)
        {
            ContainerName = "Dynamic";
        }

        public DynamicEditableLinkItemCollectionAttribute(string title, string name, int sortOrder, params Type[] pluginTypes)
            : base(title, name, sortOrder, pluginTypes)
        {
            ContainerName = "Dynamic";
        }

        public override void UpdateEditor(global::N2.ContentItem item, System.Web.UI.Control editor)
        {
            ControlUtility.FormatPropertyControl(Name, item, editor);
            base.UpdateEditor(item, editor);
        }
    }
}
