using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Web.Http;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.AspNet.Samples.Dependency
{
	public class WebContainerFactory
	{
		public ContainerBuilder Create()
		{
			var builder = new ContainerBuilder();

			var services = new ServiceCollection();
			ConfigureMicrosoftExtensions(services);
			ConfigureUmbrella(services);

			builder.Populate(services);

			// Autofac MVC registrations.
			builder.RegisterControllers(typeof(WebContainerFactory).Assembly);
			builder.RegisterModelBinders(typeof(WebContainerFactory).Assembly);
			builder.RegisterModelBinderProvider();
			builder.RegisterModule<AutofacWebTypesModule>();
			builder.RegisterFilterProvider();

			// Autofac Web API registrations
			builder.RegisterApiControllers(typeof(WebContainerFactory).Assembly);
			builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
			builder.RegisterWebApiModelBinderProvider();

			return builder;
		}

		private void ConfigureMicrosoftExtensions(IServiceCollection services)
		{
			services.AddMemoryCache();
			services.AddDistributedMemoryCache();
			services.AddLogging(x => x.SetMinimumLevel(LogLevel.Trace));
		}

		private void ConfigureUmbrella(IServiceCollection services)
		{
			services.AddUmbrellaUtilities(
				hybridCacheOptionsBuilder: (serviceProvider, options) =>
			{
				options.CacheEnabled = true;
			});

			services.AddUmbrellaWebUtilities(
				frontEndCompressionMiddlewareOptionsBuilder: (serviceProvider, options) =>
				{
					options.Mappings = new List<FrontEndCompressionMiddlewareMapping>
					{
						new FrontEndCompressionMiddlewareMapping
						{
							AppRelativeFolderPaths = new[] { "/content" },
							WatchFiles = true
						}
					};
				},
				bundleUtilityOptionsBuilder: (serviceProvider, options) =>
				{
					options.DefaultBundleFolderAppRelativePath = "/content";
					options.WatchFiles = true;
				},
				webpackBundleUtilityOptionsBuilder: (serviceProvider, options) =>
				{
					options.DefaultBundleFolderAppRelativePath = "/content/dist";
					options.WatchFiles = true;
				});

			services.AddUmbrellaLegacyWebUtilities();
		}
	}
}