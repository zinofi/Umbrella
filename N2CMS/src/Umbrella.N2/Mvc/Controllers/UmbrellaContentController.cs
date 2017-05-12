using log4net;
using Microsoft.Extensions.Logging;
using N2;
using N2.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;

namespace Umbrella.N2.Mvc.Controllers
{
    public class UmbrellaContentController<T> : ContentController<T> where T : ContentItem
    {
        #region Protected Properties
        protected ILogger Log { get; }
        #endregion

        #region Constructors
        public UmbrellaContentController(ILogger logger)
        {
            Log = logger;
        }
        #endregion
    }
}