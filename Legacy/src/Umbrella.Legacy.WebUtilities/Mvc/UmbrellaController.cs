using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Legacy.WebUtilities.Mvc
{
    public class UmbrellaController : Controller
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
