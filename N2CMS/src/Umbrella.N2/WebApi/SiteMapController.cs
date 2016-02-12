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
using Microsoft.Extensions.Logging;
using System.Web.Http.Description;
using Umbrella.Utilities.Extensions;

namespace Umbrella.N2.Utilities.WebApi
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("sitemap.xml")]
    public class SitemapController : UmbrellaApiController
    {
        public SitemapController(ILogger<SitemapController> logger) : base(logger)
        {
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
            catch(Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
    }
}
