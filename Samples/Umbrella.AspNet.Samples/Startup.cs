using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Umbrella.AspNet.Samples;
using Umbrella.AspNet.Samples.Dependency;
using Umbrella.Legacy.WebUtilities.Middleware;

[assembly: OwinStartup(typeof(Startup))]

namespace Umbrella.AspNet.Samples
{
    public class Startup
    {
        private ILogger _log;

        public void Configuration(IAppBuilder app)
        {
            // Autofac initialisation
            var containerFactory = new WebContainerFactory();
            var containerBuilder = containerFactory.Create();
            var container = containerBuilder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            ConfigureLogging(container);

            _log.WriteInformation(message: "Umbrella ASP.NET Samples - Application Started");

            app.UseAutofacLifetimeScopeInjector(container);

            app.UseMiddlewareFromContainer<FrontEndCompressionMiddleware>();
            
            app.UseStageMarker(PipelineStage.MapHandler);
        }

        private void ConfigureLogging(IContainer container)
        {
            var loggerFactory = container.Resolve<ILoggerFactory>();

            _log = loggerFactory.CreateLogger<Startup>();
        }
    }
}