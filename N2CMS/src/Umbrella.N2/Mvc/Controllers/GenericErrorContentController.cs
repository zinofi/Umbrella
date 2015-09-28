using N2;
using N2.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.N2.Utilities.Mvc.Controllers
{
    public abstract class GenericErrorContentController<T> : ContentController<T> where T : ContentItem
    {
        public override System.Web.Mvc.ActionResult Index()
        {
            if (CurrentItem == SiteSettings.Instance.NotFoundPageInstance)
            {
                Response.StatusCode = 404;
                Response.TrySkipIisCustomErrors = true;
            }
            else if (CurrentItem == SiteSettings.Instance.ErrorPageInstance)
            {
                Response.StatusCode = 500;
                Response.TrySkipIisCustomErrors = true;
            }

            return null;
        }
    }
}
