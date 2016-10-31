using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
    /// <summary>
    /// Serves as the base class for all MVC View Components.
    /// </summary>
    public abstract class UmbrellaViewComponent : ViewComponent
    {
        #region Protected Members
        protected ILogger Log { get; }
        #endregion

        #region Constructors
        public UmbrellaViewComponent(ILogger logger)
        {
            Log = logger;
        }
        #endregion
    }
}