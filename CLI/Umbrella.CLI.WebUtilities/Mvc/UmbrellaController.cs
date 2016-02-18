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
    }
}