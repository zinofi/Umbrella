using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.CLI.WebUtilities.Mvc.Filters;

namespace Umbrella.CLI.WebUtilities.Mvc
{
    [ValidationActionFilter]
    public abstract class UmbrellaApiController : UmbrellaController
    {
        public UmbrellaApiController(ILogger logger)
            : base(logger)
        {
        }
    }
}