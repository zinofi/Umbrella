using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Umbrella.Legacy.WebUtilities.Controls
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EmbeddedUserControlOptionsAttribute : Attribute
    {
        #region Public Properties
        public bool EnableViewState { get; set; }
        public ClientIDMode ClientIDMode { get; set; }
        #endregion

        #region Constructors
        public EmbeddedUserControlOptionsAttribute(bool enableViewState, ClientIDMode clientIdMode)
        {
            EnableViewState = enableViewState;
            ClientIDMode = clientIdMode;
        }
        #endregion
    }
}
