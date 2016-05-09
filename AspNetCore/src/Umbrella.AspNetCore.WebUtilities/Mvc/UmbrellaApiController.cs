using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc.Filters;
using Microsoft.AspNet.Mvc;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
    /// <summary>
    /// Serves as the base class for API controllers and encapsulates API specific functionality.
    /// </summary>
    [ServiceFilter(typeof(ValidateModelStateAttribute))]
    public abstract class UmbrellaApiController : UmbrellaController
    {
        #region Constructors
        public UmbrellaApiController(ILogger logger)
            : base(logger)
        {
        }
        #endregion

        #region Protected Methods
        protected IActionResult NoContent() => new NoContentResult();
        protected IActionResult Forbidden(string message = null) => HttpObjectOrStatusResult(message, 403);
        protected IActionResult Conflict(string message = null) => HttpObjectOrStatusResult(message, 409);
        protected IActionResult InternalServerError(string message = null) => HttpObjectOrStatusResult(message, 500, true);

        protected IActionResult HttpObjectOrStatusResult(string message, int statusCode, bool wrapMessage = false)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                object value = message;

                if (wrapMessage)
                    value = new { message };

                return new ObjectResult(value) { StatusCode = statusCode };
            }

            return new HttpStatusCodeResult(statusCode);
        }
        #endregion
    }
}