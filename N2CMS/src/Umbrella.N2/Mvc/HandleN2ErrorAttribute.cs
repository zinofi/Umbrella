using log4net;
using N2;
using N2.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Umbrella.N2.Mvc.Controllers;

namespace Umbrella.N2.Utilities.Mvc
{
    public class HandleN2ErrorAttribute : HandleErrorAttribute
    {
        #region Private Static Members
        private static readonly ILog Log = LogManager.GetLogger(typeof(HandleN2ErrorAttribute));
        #endregion

        #region Public Properties
        public string ErrorPageViewPath { get; set; }
        #endregion

        #region Constructors
        public HandleN2ErrorAttribute(string errorPageViewPath)
        {
            if (((CustomErrorsSection)WebConfigurationManager.GetSection("system.web/customErrors")).Mode == CustomErrorsMode.On)
            {
                var controllerFactory = global::N2.Context.Current.Resolve<ControllerFactoryConfigurator>()
                .NotFound<GenericErrorController>(x => x.Index())
                .ControllerFactory;

                ControllerBuilder.Current.SetControllerFactory(controllerFactory);
            }

            ErrorPageViewPath = errorPageViewPath;
        }
        #endregion

        #region Overrides
        public override void OnException(ExceptionContext filterContext)
        {
            Log.Error(filterContext.Exception.Message, filterContext.Exception);

            if (((CustomErrorsSection)WebConfigurationManager.GetSection("system.web/customErrors")).Mode == CustomErrorsMode.On)
            {
                if (filterContext.Exception.InnerException != null)
                    Log.Error(filterContext.Exception.InnerException.Message, filterContext.Exception.InnerException);

                filterContext.ExceptionHandled = true;

                ContentItem errorPage = SiteSettings.Instance.ErrorPageInstance as ContentItem;

                ViewResult result = new ViewResult
                {
                    ViewName = ErrorPageViewPath,
                    ViewData = new ViewDataDictionary(errorPage)
                };

                filterContext.Result = result;

                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.HttpContext.Response.StatusCode = 500;
            }
        }
        #endregion
    }
}