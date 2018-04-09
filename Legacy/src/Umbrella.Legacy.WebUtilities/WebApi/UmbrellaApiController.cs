using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
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
        protected StatusCodeResult Created() => new StatusCodeResult(HttpStatusCode.Created, this);
        protected ResponseMessageResult NotFound(string reason) => CreateStringContentResult(HttpStatusCode.NotFound, reason);
        protected ResponseMessageResult Conflict(string reason) => CreateStringContentResult(HttpStatusCode.Conflict, reason);
        protected ResponseMessageResult InternalServerError(string reason) => CreateStringContentResult(HttpStatusCode.InternalServerError, reason);
        protected ResponseMessageResult Unauthorized(string reason) => CreateStringContentResult(HttpStatusCode.Unauthorized, reason);
        protected ResponseMessageResult Forbidden(string reason) => CreateStringContentResult(HttpStatusCode.Forbidden, reason);
        protected ResponseMessageResult MethodNotAllowed(string reason) => CreateStringContentResult(HttpStatusCode.MethodNotAllowed, reason);
        protected StatusCodeResult NoContent() => new StatusCodeResult(HttpStatusCode.NoContent, this);
        protected ResponseMessageResult TooManyRequests(string reason) => CreateStringContentResult((HttpStatusCode)429, reason);
        protected ResponseMessageResult CreateStringContentResult(HttpStatusCode statusCode, string reason)
        {
            return new ResponseMessageResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(reason, Encoding.UTF8, "text/plain")
            });
        }
        #endregion
    }
}