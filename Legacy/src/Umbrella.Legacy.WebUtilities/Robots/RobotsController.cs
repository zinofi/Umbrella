using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Umbrella.Legacy.WebUtilities.Robots;
using Umbrella.Legacy.WebUtilities.WebApi;
using System.Web.Http.Description;
using Umbrella.WebUtilities.Robots;
using System.Web.Configuration;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(RobotsController), "RegisterRoutes")]

namespace Umbrella.Legacy.WebUtilities.Robots
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RobotsController : UmbrellaApiController
    {
        private const string c_RobotsNoIndex = "User-agent: *\r\nDisallow: /";

        public static void RegisterRoutes()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "RobotsRoute",
                routeTemplate: "robots.txt",
                defaults: new { controller = "Robots" }
            );
        }

        public IHttpActionResult Get()
        {
            try
            {
                HttpResponseMessage message = new HttpResponseMessage();

                string unRootedVirtualPath = Request.RequestUri.AbsolutePath.Replace(HttpRuntime.AppDomainAppVirtualPath, string.Empty);
                string virtualPath = "~/" + unRootedVirtualPath.TrimStart(new[] { '/' });
                string absolutePath = HostingEnvironment.MapPath(virtualPath);
                string directoryName = new DirectoryInfo(Path.GetDirectoryName(absolutePath).ToLower()).Name;

                //Firstly, determine the hostname of the site
                string hostName = Request.RequestUri.Host;

                System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~/web.config");
                RobotsConfig robotsConfig = new RobotsConfig(config);

                //Check if there is a config entry for this hostname in the robots section
                if (robotsConfig.Settings != null)
                {
                    List<RobotElement> lstRobots = robotsConfig.Settings.Robots.OfType<RobotElement>().ToList();

                    RobotElement robot = lstRobots.FirstOrDefault(x => x.HostName.ToLower() == hostName);
                    if (robot != null)
                    {
                        //Check if the element specifies the name of a robot file to use
                        string fileName = !string.IsNullOrEmpty(robot.FileName) ? robot.FileName : "robots.txt";

                        //Now we need to see if the file actually exists
                        absolutePath = Path.GetDirectoryName(absolutePath).ToLower() + @"\" + fileName;
                        if (File.Exists(absolutePath))
                        {
                            string content = null;

                            using (StreamReader reader = new StreamReader(absolutePath))
                            {
                                content = reader.ReadToEnd();
                            }

                            message.Content = new StringContent(content);
                            return ResponseMessage(message);
                        }
                    }
                }

                //If we get this far, we don't have a robots entry on disk
                //Render the default no index string
                message.Content = new StringContent(c_RobotsNoIndex);

                return ResponseMessage(message);
            }
            catch(Exception exc) when (LogError(exc))
            {
                throw;
            }
        }
    }
}
