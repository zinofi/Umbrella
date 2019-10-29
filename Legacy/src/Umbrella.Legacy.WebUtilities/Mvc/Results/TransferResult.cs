using System;
using System.Web;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Results
{
	public class TransferResult : ActionResult
	{
		public string Url { get; private set; }

		public TransferResult(string url)
		{
			Url = url;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

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
