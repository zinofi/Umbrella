using System;
using Microsoft.AspNet.Mvc;
using System.Runtime.CompilerServices;
using Umbrella.Utilities.Extensions;
using Microsoft.Extensions.Logging;

namespace Umbrella.CLI.WebUtilities.Mvc
{
    public class UmbrellaController : Controller
    {
        #region Protected Members
        protected readonly ILogger Log;
        #endregion

        #region Constructors
        public UmbrellaController(ILogger logger)
        {
            Log = logger;
        }
        #endregion

        #region Protected Methods
        protected bool LogError(Exception exc, object model = null, string message = "", bool returnValue = false, [CallerMemberName]string methodName = "") => Log.LogError(exc, model, message, returnValue, methodName);
        #endregion
    }
}