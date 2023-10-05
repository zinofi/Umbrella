// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.DataAnnotations;

/// <summary>
/// An extension of the <see cref="RemoteAttribute"/> to allow a WebAPI controller
/// to be used as the validation endpoint.
/// </summary>
/// <seealso cref="RemoteAttribute" />
public class RemoteWebApiAttribute : RemoteAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RemoteWebApiAttribute"/> class.
	/// </summary>
	/// <param name="controller">The controller.</param>
	/// <param name="routeName">Name of the route.</param>
	public RemoteWebApiAttribute(string controller, string routeName = "DefaultApi")
	{
		RouteName = routeName;
		RouteData.Add("httproute", "");
		RouteData.Add("controller", controller);
	}
}