using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Umbrella.AspNetCore.WebUtilities.Razor.TagHelpers.Filters
{
	/// <summary>
	/// A resource filter that will redirect the request to the specified action when the current request is not authenticated.
	/// </summary>
	/// <seealso cref="Attribute" />
	/// <seealso cref="IResourceFilter" />
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class OnlyAnonymousRedirectToActionAttribute : Attribute, IResourceFilter
	{
		/// <summary>
		/// Gets or sets the name of the action.
		/// </summary>
		public string ActionName { get; set; } = "Index";

		/// <summary>
		/// Gets or sets the name of the controller.
		/// </summary>
		public string ControllerName { get; set; } = "Home";

		/// <inheritdoc />
		public void OnResourceExecuted(ResourceExecutedContext context)
		{
		}

		/// <inheritdoc />
		public void OnResourceExecuting(ResourceExecutingContext context)
		{
			if (context.HttpContext.User.Identity?.IsAuthenticated is true)
				context.Result = new RedirectToActionResult(ActionName, ControllerName, null);
		}
	}
}