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
            ImageGalleryEditorControl ctrl = Page.Form.FindControl("ucImageGalleryEditorControl") as ImageGalleryEditorControl;
            if (ctrl == null)
            {
                ctrl = new ImageGalleryEditorControl();
                ctrl.ID = "ucImageGalleryEditorControl";
                Page.Form.Controls.Add(ctrl);

                //Register stylesheets and scripts - in theory these registrations will not be duplicated
				this.Page.StyleSheet("~/Content/themes/base/all.css");

                this.Page.StyleSheet(Page.ClientScript.GetWebResourceUrl(typeof(EmbeddedUserControl), "Umbrella.WebUtilities.GlobalResources.DialogModal.css"));
                this.Page.JavaScript(Page.ClientScript.GetWebResourceUrl(typeof(EmbeddedUserControl), "Umbrella.WebUtilities.GlobalResources.DialogModal.js"));

                this.Page.StyleSheet(Page.ClientScript.GetWebResourceUrl(typeof(Umbrella.N2.CustomProperties.ImageGallery.Controls.ImageGallerySummaryControl), "Umbrella.N2.CustomProperties.ImageGallery.Resources.ImageGallery.css"));
                this.Page.JavaScript(Page.ClientScript.GetWebResourceUrl(typeof(Umbrella.N2.CustomProperties.ImageGallery.Controls.ImageGallerySummaryControl), "Umbrella.N2.CustomProperties.ImageGallery.Resources.ImageGallery.js"));

                this.Page.JavaScript("~/N2/Resources/Js/UrlSelection.js");
                this.Page.JavaScript("~/Scripts/handlebars.min.js");
                this.Page.JavaScript("~/Scripts/infieldlabel.js");

                //Set the Virtual Application Url Prefix JavaScript value
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "VirtualApplicationUrlPrefix", string.Format("virtualAppUrlPrefix = \"{0}\";", HttpRuntime.AppDomainAppVirtualPath), true);
            }
        }
        #endregion

        protected override void CreateChildControls()
        {
            m_SummaryControl = new ImageGallerySummaryControl();
            m_SummaryControl.ID = "ucImageGallerySummaryControl";

            this.Controls.Add(m_SummaryControl);
        }
    }
}
