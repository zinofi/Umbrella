using N2;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Mvc.Results;
using Umbrella.N2.BaseModels;

namespace Umbrella.N2.Utilities.Mvc.Controllers
{
    public partial class GenericErrorController : Controller
    {
        public virtual ActionResult Index()
        {
            //Need to find the 404 error page
            ContentItem errorPage = SiteSettings.Instance.NotFoundPageInstance;

            HttpContext.Response.TrySkipIisCustomErrors = true;
            HttpContext.Response.StatusCode = 404;
            string url = errorPage.FindPath(PathData.DefaultAction).GetRewrittenUrl();
            return new TransferResult(url);
        }
    }
}
