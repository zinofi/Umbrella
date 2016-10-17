using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using Umbrella.N2.Extensions;

namespace Umbrella.N2.DynamicProperties.Utilities
{
    public static class ControlUtility
    {
        public static void FormatPropertyControl(string name, ContentItem item, Control editor)
        {
            Label label = editor.FindControl("lbl" + name) as Label;

            Control container = label.Parent;

            //if (container.Controls.OfType<Control>().FirstOrDefault(x => x.ID == container.ID + "_hlInheritedPage") == null)
            //{
                ContentItem inheritedFrom = item.HasInheritedDynamicValue(name);
                if (inheritedFrom == null || inheritedFrom == item)
                {
                    label.ForeColor = Color.Green;
                    label.ToolTip = "This property does not have an inherited value";
                }
                else
                {
                    label.ForeColor = Color.Red;

                    HyperLink hyperlink = new HyperLink();
                    hyperlink.NavigateUrl = Context.Current.ManagementPaths.GetEditExistingItemUrl(inheritedFrom) + "#Frame_Content_ie_Dynamic";
                    hyperlink.ToolTip = "This property inherits its value from " + inheritedFrom.Title + " -  #" + inheritedFrom.ID;
                    hyperlink.ImageUrl = hyperlink.ResolveUrl("~/N2/Resources/icons/note.png");

                    int index = container.Controls.IndexOf(label);

                    container.Controls.AddAt(index + 1, hyperlink);
                }
            //}
        }
    }
}
