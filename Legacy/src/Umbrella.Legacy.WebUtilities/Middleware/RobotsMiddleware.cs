using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using Umbrella.Legacy.WebUtilities.Robots;
using Umbrella.Utilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.Middleware
{
    /// <summary>
    /// Owin Middleware to handle requests for robots.txt and send the correct response depending on the configuration in web.config.
    /// </summary>
    /// <seealso cref="Microsoft.Owin.OwinMiddleware" />
    public class RobotsMiddleware : OwinMiddleware
    {
        private static readonly string _cacheKeyPrefix = $"{typeof(RobotsMiddleware).FullName}:Robots";
        private const string c_RobotsNoIndex = "User-agent: *\r\nDisallow: /";

        protected ILogger Log { get; }
        protected IUmbrellaHostingEnvironment HostingEnvironment { get; }
        protected IMemoryCache Cache { get; }

        public RobotsMiddleware(
            OwinMiddleware next,
            ILogger<RobotsMiddleware> logger,
            IUmbrellaHostingEnvironment hostingEnvironment,
            IMemoryCache cache)
            : base(next)
        {
            Log = logger;
            HostingEnvironment = hostingEnvironment;
            Cache = cache;
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                if (!context.Request.Path.Value.Equals("/robots.txt", StringComparison.OrdinalIgnoreCase))
                {
                    await Next.Invoke(context);
                    return;
                }

                // Firstly, determine the hostname of the site
                string hostName = context.Request.Host.Value;

                string robotsText = Cache.GetOrCreate($"{_cacheKeyPrefix}:{hostName}", entry =>
                {
                    entry.SetPriority(CacheItemPriority.NeverRemove);

                    System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~/web.config");
                    RobotsConfig robotsConfig = new RobotsConfig(config);

                    // Check if there is a config entry for this hostname in the robots section
                    if (robotsConfig.Settings != null)
                    {
                        List<RobotElement> lstRobots = robotsConfig.Settings.Robots.OfType<RobotElement>().ToList();

                        RobotElement robot = lstRobots.FirstOrDefault(x => x.HostName.ToLower() == hostName);
                        if (robot != null)
                        {
                            string fileName = !string.IsNullOrWhiteSpace(robot.FileName) ? robot.FileName : "robots.txt";
                            string path = HostingEnvironment.MapPath(fileName);

                            if (File.Exists(path))
                                return File.ReadAllText(path);
                        }
                    }

                    return string.Empty;

                });

                context.Response.ContentType = "text/plain";

                if (!string.IsNullOrWhiteSpace(robotsText))
                {
                    await context.Response.WriteAsync(robotsText);
                    return;
                }

                await context.Response.WriteAsync(c_RobotsNoIndex);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
    }
}