using N2;
using N2.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.N2.Utilities;

namespace Umbrella.N2.Mvc.Controllers
{
    public abstract class GenericErrorContentController<T> : UmbrellaContentController<T> where T : ContentItem
    {
        public override System.Web.Mvc.ActionResult Index()
        {
            try
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
            catch(Exception exc) when (LogError(exc))
            {
                throw;
            }
        }
    }
}
