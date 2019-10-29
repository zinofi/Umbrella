using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Umbrella.Legacy.WebUtilities.Mvc.Results
{
	public class TransferToRouteResult : ActionResult
	{
		public string RouteName { get; set; }
		public RouteValueDictionary RouteValues { get; set; }

		public TransferToRouteResult(RouteValueDictionary routeValues)
			: this(null, routeValues)
		{
		}

		public TransferToRouteResult(string routeName, RouteValueDictionary routeValues)
		{
			RouteName = routeName ?? string.Empty;
			RouteValues = routeValues ?? new RouteValueDictionary();
		}

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			var urlHelper = new UrlHelper(context.RequestContext);
			string url = urlHelper.RouteUrl(RouteName, RouteValues);

			var actualResult = new TransferResult(url);
			actualResult.ExecuteResult(context);
		}
	}
}