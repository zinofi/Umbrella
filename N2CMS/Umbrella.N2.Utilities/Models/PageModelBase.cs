using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Details;
using N2.Web.UI;
using N2.Persistence;
using N2.Definitions;
using N2.Collections;
using System.Diagnostics;
using Umbrella.N2.BaseModels.Enumerations;
using Umbrella.N2.Extensions;
using System.Web.UI.WebControls;

namespace Umbrella.N2.BaseModels
{
    /// <summary>
    /// A base class containing common functionality across all N2 pages. All pages on the site must inherit from this class.
    /// </summary>
    [WithEditableTitle("Page Name", 0)]
    [WithEditableName(ContainerName = ContainerNames.Metadata)]
    [WithEditableVisibility(ContainerName = ContainerNames.Metadata)]
    [WithEditablePublishedRange(ContainerName = ContainerNames.Advanced, SortOrder = 1)]
    [SidebarContainer(ContainerNames.Metadata, 100, HeadingText = "Metadata")]
    [TabContainer(ContainerNames.Content, "Content", 1)]
    [TabContainer(ContainerNames.Advanced, "Advanced", 2)]
    [TabContainer(ContainerNames.Header, "Header", 3)]
    [TabContainer(ContainerNames.Footer, "Footer", 4)]
    [TabContainer(ContainerNames.Social, "Social", 5)]
    [TabContainer(ContainerNames.Facebook, "Facebook", 6)]
    [TabContainer(ContainerNames.Dynamic, "Dynamic Properties", 9999)]
    [SortChildren(SortBy.CurrentOrder)]
    [DebuggerDisplay("{ID} - {Name}")]
    public abstract class PageModelBase : ContentItem
    {
        /// <summary>
        /// Used to specifiy a custom sorting mechanism
        /// </summary>
        [Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableSortOrder("Custom Sort Method", 1, ContainerName = ContainerNames.Advanced)]
        public virtual CustomSortBy CustomSortMethod
        {
            get
            {
                int? value = this.GetPropertyValue(x => (int?)x.CustomSortMethod);
                return value.HasValue ? (CustomSortBy)value.Value : CustomSortBy.SortOrder;
            }
            set
            {
                this.SetPropertyValue(x => x.CustomSortMethod, (int?)value);
            }
        }

        #region Helper Properties

        /// <summary>
        /// Returns the absolute URL for the current item
        /// </summary>
        public string AbsoluteUrl
        {
            get { return Find.RootItem.Details["Installation.Host"].StringValue + Url; }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Gets the value of a property using a specified fallback string if the value is null or empty.
        /// The fallback value is only used outside of N2 edit mode to avoid the fallback being saved to a page property.
        /// </summary>
        /// <param name="value">The value of the property</param>
        /// <param name="fallback">The fallback value (only applicable outside N2 edit mode)</param>
        /// <returns>The value, if it exists, or the fallback (if applicable)</returns>
        protected string GetPropertyValueWithFallback(string value, string fallback)
        {
            //We need to make sure that a value is only returned if we are not in edit mode
            string path = Context.Current.RequestContext.Url.Path;

            return !string.IsNullOrEmpty(value)
                    ? value
                    : !path.StartsWith(Context.Current.ManagementPaths.GetManagementInterfaceUrl())
                        ? fallback
                        : null;
        }
        #endregion
    }
}