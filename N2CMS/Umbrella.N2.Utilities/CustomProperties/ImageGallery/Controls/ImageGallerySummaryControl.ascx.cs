using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Resources;
using Umbrella.Legacy.WebUtilities.Controls;

namespace Umbrella.N2.CustomProperties.ImageGallery.Controls
{
    [EmbeddedUserControlOptions(false, ClientIDMode.Predictable)]
    public partial class ImageGallerySummaryControl : EmbeddedUserControl
    {
        #region Control Declarations
        protected global::System.Web.UI.WebControls.Panel pnlImageGallerySummary;
        protected global::System.Web.UI.WebControls.HiddenField hdnJSONData;
        protected global::System.Web.UI.WebControls.Literal litImagesCount;
        #endregion

        #region Public Properties
        public string Value
        {
            get
            {
                return !string.IsNullOrEmpty(hdnJSONData.Value)
                    ? hdnJSONData.Value
                    : null;
            }
            set
            {
                hdnJSONData.Value = value;
            }
        }

        public int Count
        {
            set
            {
                litImagesCount.Text = value.ToString();
            }
        }
        #endregion

        protected void Page_PreRender(object sender, EventArgs e)
        {
            pnlImageGallerySummary.Attributes.Add("data-upload-folder-url", ResolveUrl("/api/ImageGalleryUploadFolder"));
            pnlImageGallerySummary.Attributes.Add("data-upload-file-url", ResolveUrl("/api/ImageGalleryUploadFile"));
        }
    }
}