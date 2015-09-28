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
    [EmbeddedUserControlOptions(false, ClientIDMode.Static)]
    public partial class ImageGalleryEditorControl : EmbeddedUserControl
    {
        #region Control Declarations
        protected global::System.Web.UI.WebControls.Literal litEditIconUrl;
        protected global::System.Web.UI.WebControls.Literal litDeleteIconUrl;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            litEditIconUrl.Text = ResolveUrl("~/N2/Resources/icons/pencil.png");
            litDeleteIconUrl.Text = ResolveUrl("~/N2/Resources/icons/delete.png");
        }
    }
}