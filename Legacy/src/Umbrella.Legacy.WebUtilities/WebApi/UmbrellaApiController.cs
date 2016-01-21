using log4net;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Results;
using Umbrella.Legacy.WebUtilities.WebApi.Filters;
using Umbrella.Utilities.Log4Net;

namespace Umbrella.Legacy.WebUtilities.WebApi
{
    [ValidationActionFilter]
    public abstract class UmbrellaApiController : ApiController
    {
        #region Protected Static Members
        protected static readonly ILog Log = LogManager.GetLogger(typeof(UmbrellaApiController));
        #endregion

        #region Protected Methods
        protected NegotiatedContentResult<string> NotFound(string reason) => Content(HttpStatusCode.NotFound, reason);
        protected NegotiatedContentResult<string> Conflict(string reason) => Content(HttpStatusCode.Conflict, reason);
        protected NegotiatedContentResult<string> InternalServerError(string reason) => Content(HttpStatusCode.InternalServerError, reason);
        protected NegotiatedContentResult<string> Unauthorized(string reason) => Content(HttpStatusCode.Unauthorized, reason);
        protected NegotiatedContentResult<string> Forbidden(string reason) => Content(HttpStatusCode.Forbidden, reason);
        protected NegotiatedContentResult<string> MethodNotAllowed(string reason) => Content(HttpStatusCode.MethodNotAllowed, reason);
        protected StatusCodeResult NoContent() => new StatusCodeResult(HttpStatusCode.NoContent, this);
        protected bool LogError(Exception exc, object model = null, string message = "", bool returnValue = false, [CallerMemberName]string methodName = "") => Log.LogError(exc, model, message, returnValue, methodName);
        #endregion
    }
}