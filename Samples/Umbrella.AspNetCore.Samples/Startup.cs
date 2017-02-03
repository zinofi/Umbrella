using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities;
using Umbrella.Utilities;
using Umbrella.AspNetCore.DataAnnotations;
using Umbrella.Extensions.Logging.Log4Net;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Umbrella.AspNetCore.Samples
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddLogging();
            services.AddOptions();
            
            services.AddMvc();
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            //Umbrella Services
            services.AddUmbrellaAspNetCoreDataAnnotations();
            services.AddUmbrellaAspNetCoreWebUtilities();
            services.AddUmbrellaUtilities();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            bool isDevelopment = env.IsDevelopment();
            bool isProduction = env.IsProduction();

            string log4netConfigFileName = "Log4Net.config";

            if (isProduction)
            {
                log4netConfigFileName = "Log4Net.production.config";
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddUmbrellaLog4Net(env.ContentRootPath, log4netConfigFileName);
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Error);

            var logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("Umbrella AspNetCore Samples Application Started");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //Force the Culture being used to be en-GB. We never want anything else to be used.
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-GB", "en-GB"),
                SupportedCultures = new[] { new CultureInfo("en-GB") },
                SupportedUICultures = new[] { new CultureInfo("en-GB") }
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
