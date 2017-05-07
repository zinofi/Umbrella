using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace System.Web.Mvc
{
	public static class UrlHelperExtensions2
	{
		public static string RouteWebApiUrl(this UrlHelper helper, string controller, IDictionary<string, object> values = null, string routeName = "DefaultApi")
		{
			if (values == null)
				values = new Dictionary<string, object>();

			values.Add("httproute", "");
			values.Add("controller", controller);

			return helper.RouteUrl(routeName, new RouteValueDictionary(values));
		}
	}
}