using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Umbrella.N2.CustomProperties.LinkEditor.Extensions;
using System.Reflection;
using Umbrella.N2.CustomProperties.LinkEditor.Controls.Templates;
using Umbrella.Legacy.WebUtilities.Controls;

namespace Umbrella.N2.CustomProperties.LinkEditor.Controls
{
    /// <summary>
    /// There should only ever be a single instance of this control per edit page
    /// </summary>
    [EmbeddedUserControlOptions(false, ClientIDMode.Static)]
    public partial class LinkListPopup : EmbeddedUserControl
    {
        #region Control Declarations
        protected PlaceHolder plhButtons;
        #endregion
    }
}