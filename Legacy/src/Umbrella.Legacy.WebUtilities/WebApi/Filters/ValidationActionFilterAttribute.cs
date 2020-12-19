using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Umbrella.Legacy.WebUtilities.WebApi.Filters
{
	/// <summary>
	/// A Web API action filter that return a 400 BadRequest response containing model state errors if the model state is not valid.
	/// </summary>
	/// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute" />
	public class ValidationActionFilterAttribute : ActionFilterAttribute
	{
		/// <inheritdoc />
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			var modelState = actionContext.ModelState;

			if (!modelState.IsValid)
				actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, modelState);
		}
	}
}