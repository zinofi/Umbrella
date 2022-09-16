using CommunityToolkit.Diagnostics;
using System;
using System.Web;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Results
{
	/// <summary>
	/// An <see cref="ActionResult"/> used to rewrite the current request to the specified <see cref="Url"/>.
	/// </summary>
	/// <seealso cref="System.Web.Mvc.ActionResult" />
	public class TransferResult : ActionResult
	{
		/// <summary>
		/// Gets the target URL.
		/// </summary>
		public string Url { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TransferResult"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		public TransferResult(string url)
		{
			Url = url;
		}

		/// <inheritdoc />
		public override void ExecuteResult(ControllerContext context)
		{
			Guard.IsNotNull(context);

			var httpContext = HttpContext.Current;

			// MVC 3+ running on IIS 7+
			if (HttpRuntime.UsingIntegratedPipeline)
			{
				httpContext.Server.TransferRequest(Url, true);
			}
			else
			{
				// Pre MVC 3
				httpContext.RewritePath(Url, false);

				IHttpHandler httpHandler = new MvcHttpHandler();
				httpHandler.ProcessRequest(httpContext);
			}
		}
	}
}