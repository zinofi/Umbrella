using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Umbrella.Legacy.WebUtilities.WebApi.Routing.Constraints
{
    public class OutboundLowercaseRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.IncomingRequest)
                return true;

            var path = httpContext.Request.Url.AbsolutePath;

            return path.Equals(path.ToLowerInvariant(), StringComparison.InvariantCulture);
        }
    }
}