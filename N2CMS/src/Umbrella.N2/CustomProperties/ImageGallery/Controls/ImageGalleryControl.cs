using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Resources;
using Umbrella.Legacy.WebUtilities.Controls;
using Umbrella.Legacy.WebUtilities.Extensions;
using N2.Web.UI.WebControls;

namespace Umbrella.N2.CustomProperties.ImageGallery.Controls
{
    public class ImageGalleryControl : WebControl, INamingContainer
    {
        private ImageGallerySummaryControl m_SummaryControl;

        public string Value
        {
            get { return m_SummaryControl.Value; }
        }

        #region Public Methods
        public void Initialize(string json, int count)
        {
            EnsureChildControls();

            m_SummaryControl.Count = count;
            m_SummaryControl.Value = json;

            //We need to ensure that there is only a single instance of this control per edit page!
            ImageGalleryEditorControl ctrl = Page.FindFirstControl<ItemEditor>().FindControl("ucImageGalleryEditorControl") as ImageGalleryEditorControl;
            if (ctrl == null)
            {
                ctrl = new ImageGalleryEditorControl
                {
                    ID = "ucImageGalleryEditorControl"
                };
                Page.FindFirstControl<ItemEditor>().Controls.Add(ctrl);

                //Register stylesheets and scripts - in theory these registrations will not be duplicated
				Page.StyleSheet("~/Content/themes/base/all.css");

                Page.StyleSheet(Page.ClientScript.GetWebResourceUrl(typeof(EmbeddedUserControl), "Umbrella.Legacy.WebUtilities.GlobalResources.DialogModal.css"));
                Page.JavaScript(Page.ClientScript.GetWebResourceUrl(typeof(EmbeddedUserControl), "Umbrella.Legacy.WebUtilities.GlobalResources.DialogModal.js"));

                Page.StyleSheet(Page.ClientScript.GetWebResourceUrl(typeof(ImageGallerySummaryControl), "Umbrella.N2.CustomProperties.ImageGallery.Resources.ImageGallery.css"));
                Page.JavaScript(Page.ClientScript.GetWebResourceUrl(typeof(ImageGallerySummaryControl), "Umbrella.N2.CustomProperties.ImageGallery.Resources.ImageGallery.js"));

                Page.JavaScript("~/N2/Resources/Js/UrlSelection.js");
                Page.JavaScript("~/Scripts/handlebars.min.js");

                //Set the Virtual Application Url Prefix JavaScript value
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "VirtualApplicationUrlPrefix", string.Format("virtualAppUrlPrefix = \"{0}\";", HttpRuntime.AppDomainAppVirtualPath), true);
            }
        }
        #endregion

        protected override void CreateChildControls()
        {
            m_SummaryControl = new ImageGallerySummaryControl
            {
                ID = "ucImageGallerySummaryControl"
            };

            Controls.Add(m_SummaryControl);
        }
    }
}
