using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using N2;
using Umbrella.N2.DynamicProperties.Utilities;

namespace Umbrella.N2.DynamicProperties
{
    public class DynamicEditableTextAttribute : EditableTextAttribute
    {
        #region Constructors
        public DynamicEditableTextAttribute()
        {
            ContainerName = "Dynamic";
        }

        public DynamicEditableTextAttribute(string title, int sortOrder)
            : this()
        {
            Title = title;
            SortOrder = sortOrder;
        }
        #endregion

        #region Overridden Methods
        public override void UpdateEditor(global::N2.ContentItem item, Control editor)
        {
            ControlUtility.FormatPropertyControl(Name, item, editor);
            base.UpdateEditor(item, editor);
        }
        #endregion
    }
}
