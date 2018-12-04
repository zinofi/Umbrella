using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class EnableCorsAttribute : ActionFilterAttribute
    {
        public bool AllowCredentials { get; set; }
        public string AllowOrigin { get; set; }
        public string AllowHeaders { get; set; }
        public string AllowMethods { get; set; }
        public string ExposeHeaders { get; set; }
        public int MaxAgeSeconds { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var response = filterContext.RequestContext.HttpContext.Response;

            void TryAddStringHeader(string name, string value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    response.AddHeader(name, value);
            }

            if (AllowCredentials)
                response.AddHeader("Access-Control-Allow-Credentials", "true");

            if (MaxAgeSeconds > 0)
                response.AddHeader("Access-Control-Max-Age", MaxAgeSeconds.ToString());

            TryAddStringHeader("Access-Control-Allow-Origin", AllowOrigin);
            TryAddStringHeader("Access-Control-Allow-Headers", AllowHeaders);
            TryAddStringHeader("Access-Control-Allow-Methods", AllowMethods);
            TryAddStringHeader("Access-Control-Expose-Headers", ExposeHeaders);

            base.OnActionExecuting(filterContext);
        }
    }
}