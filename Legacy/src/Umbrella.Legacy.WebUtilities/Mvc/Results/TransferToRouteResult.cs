using System.Web.Mvc;
using System.Web.Routing;

namespace Umbrella.Legacy.WebUtilities.Mvc.Results;

/// <summary>
/// An action result that transfers the current action method to the one specified by the routing values.
/// </summary>
/// <seealso cref="ActionResult" />
public class TransferToRouteResult : ActionResult
{
	/// <summary>
	/// Gets or sets the name of the route.
	/// </summary>
	public string RouteName { get; set; }

	/// <summary>
	/// Gets or sets the route values.
	/// </summary>
	public RouteValueDictionary RouteValues { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TransferToRouteResult"/> class.
	/// </summary>
	/// <param name="routeValues">The route values.</param>
	public TransferToRouteResult(RouteValueDictionary routeValues)
		: this(null, routeValues)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TransferToRouteResult"/> class.
	/// </summary>
	/// <param name="routeName">Name of the route.</param>
	/// <param name="routeValues">The route values.</param>
	public TransferToRouteResult(string? routeName, RouteValueDictionary routeValues)
	{
		RouteName = routeName ?? string.Empty;
		RouteValues = routeValues ?? [];
	}

	/// <inheritdoc />
	public override void ExecuteResult(ControllerContext context)
	{
		if (context is null)
			throw new ArgumentNullException(nameof(context));

		var urlHelper = new UrlHelper(context.RequestContext);
		string url = urlHelper.RouteUrl(RouteName, RouteValues);

		var actualResult = new TransferResult(url);
		actualResult.ExecuteResult(context);
	}
}