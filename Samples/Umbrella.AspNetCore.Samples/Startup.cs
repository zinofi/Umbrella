using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Umbrella.DynamicImage.Caching;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.FileSystem.Disk;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Umbrella.AspNetCore.Samples
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get; }

        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

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
            services.AddUmbrellaWebUtilities();
            services.AddUmbrellaDynamicImage(new DynamicImageCacheOptions { CacheKeyCacheOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) } });
            //services.AddUmbrellaDynamicImageSoundInTheory();
            services.AddUmbrellaDynamicImageFreeImage();
            //services.AddUmbrellaDynamicImageSkiaSharp();
            //services.AddUmbrellaDynamicImageMemoryCache(new DynamicImageMemoryCacheOptions { ItemCacheOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) } });

            services.AddUmbrellaDiskFileProvider(new UmbrellaDiskFileProviderOptions { RootPhysicalPath = HostingEnvironment.WebRootPath });

            UmbrellaStatics.JsonSerializer = (obj, useCamelCase, typeNameHandling) =>
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = (TypeNameHandling)typeNameHandling
                };

                if (useCamelCase)
                {
                    jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    return JsonConvert.SerializeObject(obj, jsonSettings);
                }
                else
                    return JsonConvert.SerializeObject(obj, jsonSettings);
            };

            UmbrellaStatics.JsonDeserializer = (json, type, typeNameHandling) => JsonConvert.DeserializeObject(json, type); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IUmbrellaFileProvider fileProvider, IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

            app.UseUmbrellaDynamicImage(config => config.SourceFileProvider = fileProvider);

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
