using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umbrella.Legacy.WebUtilities.Controls;

namespace Umbrella.N2.CustomProperties.LinkEditor.Controls
{
    [EmbeddedUserControlOptions(false, ClientIDMode.Static)]
    public partial class ImageEditorPopup : EmbeddedUserControl
    {
        #region Control Declarations
        protected ValidationSummary vsAddEditImage;
        protected System.Web.UI.HtmlControls.HtmlInputText txtImageUrl;
        protected RequiredFieldValidator rfvImageUrl;
        #endregion
    }
}