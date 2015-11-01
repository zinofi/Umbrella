using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Umbrella.WebUtilities.Mvc.DataAnnotations
{
    public class RemoteWebApiAttribute : RemoteAttribute
    {
        public RemoteWebApiAttribute(string controller, string routeName = "DefaultApi")
        {
            RouteName = routeName;
            RouteData.Add("httproute", "");
            RouteData.Add("controller", controller);
        }
    }
}