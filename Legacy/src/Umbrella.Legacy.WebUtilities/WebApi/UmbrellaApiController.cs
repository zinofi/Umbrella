using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Results;
using Umbrella.Legacy.WebUtilities.WebApi.Filters;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Legacy.WebUtilities.WebApi
{
    [ValidationActionFilter]
    public abstract class UmbrellaApiController : ApiController
    {
        #region Protected Properties
        protected ILogger Log { get; }
        #endregion

        #region Constructors
        public UmbrellaApiController(ILogger logger)
        {
            Log = logger;
        }
        #endregion

        #region Protected Methods
        protected NegotiatedContentResult<string> NotFound(string reason) => Content(HttpStatusCode.NotFound, reason);
        protected NegotiatedContentResult<string> Conflict(string reason) => Content(HttpStatusCode.Conflict, reason);
        protected NegotiatedContentResult<string> InternalServerError(string reason) => Content(HttpStatusCode.InternalServerError, reason);
        protected NegotiatedContentResult<string> Unauthorized(string reason) => Content(HttpStatusCode.Unauthorized, reason);
        protected NegotiatedContentResult<string> Forbidden(string reason) => Content(HttpStatusCode.Forbidden, reason);
        protected NegotiatedContentResult<string> MethodNotAllowed(string reason) => Content(HttpStatusCode.MethodNotAllowed, reason);
        protected StatusCodeResult NoContent() => new StatusCodeResult(HttpStatusCode.NoContent, this);
        #endregion
    }
}