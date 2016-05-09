using System;
using Microsoft.AspNet.Mvc;
using System.Runtime.CompilerServices;
using Umbrella.Utilities.Extensions;
using Microsoft.Extensions.Logging;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
    /// <summary>
    /// Serves as the base class for all MVC controllers and also the <see cref="UmbrellaApiController"/>.
    /// </summary>
    public abstract class UmbrellaController : Controller
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