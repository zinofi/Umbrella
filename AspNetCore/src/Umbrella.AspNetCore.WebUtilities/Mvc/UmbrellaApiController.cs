using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelState;
using Umbrella.DataAccess.Exceptions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
    /// <summary>
    /// Serves as the base class for API controllers and encapsulates API specific functionality.
    /// </summary>
    [ServiceFilter(typeof(ValidateModelStateAttribute))]
    public abstract class UmbrellaApiController : UmbrellaController
    {
        #region Private Members
        private readonly IModelStateTransformer m_ModelStateTransformer; 
        #endregion

        #region Constructors
        public UmbrellaApiController(ILogger logger, IModelStateTransformer modelStateTransformer)
            : base(logger)
        {
            m_ModelStateTransformer = modelStateTransformer;
        }
        #endregion

        #region Overridden Methods
        public override BadRequestObjectResult BadRequest(ModelStateDictionary modelState)
        {
            return BadRequest(m_ModelStateTransformer.TransformToObject(ModelState));
        }
        #endregion

        #region Public Methods
        [NonAction]
        public virtual IActionResult Forbidden(string message = null) => HttpObjectOrStatusResult(message, 403);

        [NonAction]
        public virtual IActionResult Conflict(string message = null) => HttpObjectOrStatusResult(message, 409);

        [NonAction]
        public virtual IActionResult InternalServerError(string message = null) => HttpObjectOrStatusResult(message, 500, true);

        [NonAction]
        public virtual IActionResult HttpObjectOrStatusResult(string message, int statusCode, bool wrapMessage = false)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                object value = message;

                if (wrapMessage)
                    value = new { message };

                return StatusCode(statusCode, value);
            }

            return StatusCode(statusCode);
        }
        #endregion

        #region Protected Methods
        protected IActionResult HandleDataValidationException(DataAccessValidationException exc)
        {
            switch (exc.ValidationType)
            {
                case DataValidationType.Conflict:
                    return Conflict(exc.Message);
                case DataValidationType.Invalid:
                default:
                    return BadRequest(exc.Message);
            }
        }
        #endregion
    }
}