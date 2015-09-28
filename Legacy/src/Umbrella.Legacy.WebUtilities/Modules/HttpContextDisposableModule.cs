using System;
using System.Web;
using System.Linq;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using log4net;
using Umbrella.Legacy.WebUtilities.Modules;
using Umbrella.Legacy.WebUtilities.Extensions;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(HttpContextDisposableModule), "PreStart")]

namespace Umbrella.Legacy.WebUtilities.Modules
{
    /// <summary>
    /// The purpose of this module is clean up all disposable objects held in the HttpContext.
    /// This module is dynamically registered at runtime using the RegisterModule method.
    /// This method is called by the WebActivatorEx.PreApplicationStartMethod assembly attribute
    /// </summary>
    public class HttpContextDisposableModule : IHttpModule
    {
        private static readonly ILog s_Log = LogManager.GetLogger(typeof(HttpContextDisposableModule));

        public static void PreStart()
        {
			//TODO: This check doesn't work according to the logs
			if (!AppDomain.CurrentDomain.IsOwinApp())
			{
				DynamicModuleUtility.RegisterModule(typeof(HttpContextDisposableModule));

				if (s_Log.IsDebugEnabled)
					s_Log.Debug("HttpContextDisposableModule PreStart() method called successfully");
			}
        }

        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += (sender, args) =>
            {
                foreach (IDisposable disposable in HttpContext.Current.Items.OfType<IDisposable>())
                {
                    disposable.Dispose();

                    if (s_Log.IsDebugEnabled)
                        s_Log.Debug("HttpContextDisposableModule HttpApplication.EndRequest event handled for type " + disposable.GetType().FullName);
                }
            };

            if (s_Log.IsDebugEnabled)
                s_Log.Debug("HttpContextDisposableModule Init() method called successfully");
        }

        #endregion
    }
}
