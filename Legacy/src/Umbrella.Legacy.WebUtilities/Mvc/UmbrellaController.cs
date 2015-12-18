using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbrella.Utilities.log4net;

namespace Umbrella.Legacy.WebUtilities.Mvc
{
    public class UmbrellaController : Controller
    {
        #region Protected Static Members
        protected static readonly ILog Log = LogManager.GetLogger(typeof(UmbrellaController));
        #endregion

        #region Protected Methods
        protected bool LogError(Exception exc, object model = null, string message = "", bool returnValue = false, [CallerMemberName]string methodName = "") => Log.LogError(exc, model, message, returnValue, methodName);
        #endregion
    }
}
