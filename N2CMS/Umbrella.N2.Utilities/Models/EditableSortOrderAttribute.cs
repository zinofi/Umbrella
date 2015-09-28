using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2;
using Umbrella.N2.BaseModels.Enumerations;

namespace Umbrella.N2.BaseModels
{
    /// <summary>
    /// Attribute used to specify a current sorting mechanism for the item's children
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EditableSortOrderAttribute : EditableDropDownAttribute
    {
        /// <summary>
        /// Creates a new instance of this type
        /// </summary>
        /// <param name="title">The display name used for edit mode for any properties marked with this attribute</param>
        /// <param name="sortOrder">The sort order for any properties marked with this attribute</param>
        public EditableSortOrderAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
            Required = true;
        }

        /// <summary>
        /// Gets the list items for the drop down list
        /// </summary>
        /// <returns>A list of items</returns>
        protected override System.Web.UI.WebControls.ListItem[] GetListItems()
        {
            return Enum.GetValues(typeof(CustomSortBy)).Cast<int>().Select(x => new ListItem(((CustomSortBy)x).ToFriendlyString(), x.ToString())).ToArray();
        }

        public override void UpdateEditor(ContentItem item, System.Web.UI.Control editor)
        {
            ListControl ddl = editor as ListControl;
            if (item[Name] != null)
            {
                ddl.SelectedValue = ((int)item[Name]).ToString();
            }
        }
    }
}