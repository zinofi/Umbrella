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
    public partial class LinkEditorPopup : EmbeddedUserControl
    {
        #region Control Declarations
        protected global::System.Web.UI.WebControls.ValidationSummary vsAddEditLink;
        protected global::System.Web.UI.HtmlControls.HtmlInputText txtLinkText;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvLinkText;
        protected global::System.Web.UI.HtmlControls.HtmlInputText txtLinkImageUrl;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvLinkImageUrl;
        protected global::System.Web.UI.HtmlControls.HtmlInputText txtLinkUrl;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvLinkUrl;
        #endregion
    }
}