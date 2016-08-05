using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Umbrella.Utilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.Hosting
{
    public class UmbrellaHostingEnvironment : IUmbrellaHostingEnvironment
    {
        public string MapPath(string virtualPath, bool fromContentRoot = true)
        {
            return System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
        }

        public string MapWebPath(string virtualPath, string scheme = "http")
        {
            string baseUrl = $"{scheme}://{HttpContext.Current.Request.Url.Host}";

            string appPath = HttpRuntime.AppDomainAppVirtualPath;
            if (appPath != "/")
                baseUrl += appPath;

            return baseUrl + virtualPath.Remove(0, 1);
        }
    }
}