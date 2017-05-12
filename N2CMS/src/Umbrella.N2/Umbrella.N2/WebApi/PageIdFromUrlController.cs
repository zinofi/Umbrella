using log4net;
using N2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbrella.Legacy.WebUtilities.WebApi;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Extensions;

namespace Umbrella.N2.Utilities.WebApi
{
    [Authorize]
    public class PageIdFromUrlController : UmbrellaApiController
    {
        public PageIdFromUrlController(ILogger<PageIdFromUrlController> logger) : base(logger)
        {
        }

        public IHttpActionResult Get(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return BadRequest("The url must be specified");

                ContentItem item = Context.UrlParser.Parse(url);

                if (item != null)
                    return Ok(new { Id = item.ID, Name = item.Title });

                return NotFound("A page cannot be found with the specified url");
            }
            catch (Exception exc) when (Log.WriteError(exc, url))
            {
                throw;
            }
        }
    }
}