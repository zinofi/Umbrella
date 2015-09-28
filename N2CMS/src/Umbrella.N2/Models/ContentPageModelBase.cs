using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Persistence;
using Umbrella.N2.Extensions;

namespace Umbrella.N2.BaseModels
{
    /// <summary>
    /// A base class to be used for all content pages
    /// </summary>
    public class ContentPageModelBase : PageModelBase
    {
        /// <summary>
        /// The meta title for the current item
        /// </summary>
        //[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableText("Page Title", 10, ContainerName = ContainerNames.Metadata)]
        public virtual string PageTitle { get; set; }

        /// <summary>
        /// Determines whether or not the page title structure can be overridden by using just the meta title in its
        /// entirity from this item
        /// </summary>
        //[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableCheckBox("Page Title - Override Structure", 11, ContainerName = ContainerNames.Metadata)]
        public virtual bool PageTitleOverrideStructure { get; set; }

        /// <summary>
        /// The meta keywords for the current item
        /// </summary>
        //[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableText("Meta Keywords", 12, ContainerName = ContainerNames.Metadata)]
        public virtual string MetaKeywords { get; set; }

        /// <summary>
        /// The meta description for the current item
        /// </summary>
        //[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableText("Meta Description", 13, TextMode = TextBoxMode.MultiLine, Rows = 10, ContainerName = ContainerNames.Metadata)]
        public virtual string MetaDescription { get; set; }

        /// <summary>
        /// Determine whether this page should be excluded from search engines
        /// </summary>
		//[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableCheckBox("No Index", 14, ContainerName = ContainerNames.Metadata)]
        public virtual bool NoIndex { get; set; }

        /// <summary>
        /// Determine whether the links on this page should be crawled by search engines
        /// </summary>
		//[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
		[EditableCheckBox("No Follow", 15, ContainerName = ContainerNames.Metadata)]
        public virtual bool NoFollow { get; set; }

        /// <summary>
        /// The name used for the current item in navigation menus. Defaults to the Title property.
        /// </summary>
        //[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableText("Name In Navigation", 16, ContainerName = ContainerNames.Advanced, HelpTitle = "Help", HelpText = "Defaults to the page name")]
        public virtual string NameInNavigation
        {
            get
            {
                string value = this.GetPropertyValue(x => x.NameInNavigation);
                return GetPropertyValueWithFallback(value, this.Title);
            }
            set
            {
                this.SetPropertyValue(x => x.NameInNavigation, value);
            }
        }

        /// <summary>
        /// The name used for the current item in breadcrumbs. Defaults to the NameInNavigation property.
        /// </summary>
        //[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableText("Name In Breadcrumb", 17, ContainerName = ContainerNames.Advanced)]
        public virtual string NameInBreadcrumb
        {
            get
            {
                string value = this.GetPropertyValue(x => x.NameInBreadcrumb);
                return GetPropertyValueWithFallback(value, this.NameInNavigation);
            }
            set
            {
                this.SetPropertyValue(x => x.NameInBreadcrumb, value);
            }
        }

        /// <summary>
        /// Determines whether the current item should be hidden from the breadcrumb
        /// </summary>
        //[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableCheckBox("Hide From Breadcrumb", 18, ContainerName = ContainerNames.Advanced)]
        public virtual bool HideFromBreadcrumb { get; set; }

        /// <summary>
        /// The page heading for the current item, usually used for H1 tags. Defaults to the Title property.
        /// </summary>
        //[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableText("Page Heading", 19,
            ContainerName = ContainerNames.Content,
            HelpTitle = "Help",
            HelpText = "The Page Heading. Primarily used for the H1 tag. Defaults to the page Title property.")]
        public virtual string PageHeading
        {
            get
            {
                string value = this.GetPropertyValue(x => x.PageHeading);
                return GetPropertyValueWithFallback(value, this.Title);
            }
            set
            {
                this.SetPropertyValue(x => x.PageHeading, value);
            }
        }

        /// <summary>
        /// The main body text for the current item.
        /// </summary>
        //[Persistable(PersistAs = PropertyPersistenceLocation.Column)]
        [EditableFreeTextArea("Main Body", 20, ContainerName = ContainerNames.Content, HelpTitle = "Help", HelpText = "The main body text for the page")]
        public virtual string MainBody { get; set; }

        #region Helper Properties
        public string RobotsMetaContent
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (NoIndex)
                    sb.Append("No Index");

                if(NoFollow)
                {
                    if (NoIndex)
                        sb.Append(", ");

                    sb.Append("NoFollow");
                }

                return sb.ToString();
            }
        }
        #endregion
    }
}
