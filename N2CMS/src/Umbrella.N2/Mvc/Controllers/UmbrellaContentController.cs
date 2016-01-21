using log4net;
using N2;
using N2.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Log4Net;

namespace Umbrella.N2.Mvc.Controllers
{
    public class UmbrellaContentController<T> : ContentController<T> where T : ContentItem
    {
        #region Protected Static Members
        protected static readonly ILog Log = LogManager.GetLogger(typeof(UmbrellaContentController<T>));
        #endregion

        #region Protected Methods
        protected bool LogError(Exception exc, object model = null, string message = "", bool returnValue = false, [CallerMemberName]string methodName = "") => Log.LogError(exc, model, message, returnValue, methodName);
        #endregion
    }
}