using N2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbrella.N2.BaseModels;
using Umbrella.N2.Utilities.WebApi;
using Umbrella.WebUtilities.SiteMap;
using Umbrella.Legacy.WebUtilities.WebApi;
using Umbrella.Legacy.WebUtilities.Extensions;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SitemapController), "RegisterRoutes")]

namespace Umbrella.N2.Utilities.WebApi
{
    public class SitemapController : UmbrellaApiController
    {
        public static void RegisterRoutes()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "SitemapRoute",
                routeTemplate: "sitemap.xml",
                defaults: new { controller = "Sitemap" }
            );
        }

        public IHttpActionResult Get()
        {
            try
            {
                //Build the sitemap
                XmlSiteMap sitemap = new XmlSiteMap();

                foreach (ContentPageModelBase page in Find.Query<ContentPageModelBase>().Where(x => x.State == ContentState.Published).AsEnumerable().Where(x => !x.NoIndex))
                {
                    sitemap.Add(page.Url.ToAbsoluteUrl(), ChangeFrequency.Weekly, page.Published.Value);
                }

                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(sitemap.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

                return ResponseMessage(response);
            }
            catch(Exception exc) when (LogError(exc))
            {
                throw;
            }
        }
    }
}
