using System;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Umbrella.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
    /// <summary>
    /// Serves as the base class for all MVC controllers and also the <see cref="UmbrellaApiController"/>.
    /// </summary>
    public abstract class UmbrellaController : Controller
    {
        #region Protected Properties
        protected ILogger Log { get; }
        #endregion

        #region Constructors
        public UmbrellaController(ILogger logger)
        {
            Log = logger;
        }
        #endregion
    }
}