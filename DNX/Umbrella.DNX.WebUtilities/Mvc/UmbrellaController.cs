using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using log4net;
using System.Runtime.CompilerServices;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DNX.WebUtilities.Mvc
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