using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Umbrella.WebUtilities.Robots;
using System.Web.Configuration;

namespace Umbrella.Legacy.WebUtilities.Robots
{
    /// <summary>
    /// This handler intercepts requests for the robots.txt file and depending on the robots config
    /// entries in web.config will return different content.
    /// </summary>
    public class RobotsHandler : IHttpHandler
    {
        private const string c_RobotsNoIndex = "User-agent: *\r\nDisallow: /";

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string unRootedVirtualPath = context.Request.Url.AbsolutePath.Replace(HttpRuntime.AppDomainAppVirtualPath, string.Empty);
            string virtualPath = "~/" + unRootedVirtualPath.TrimStart(new[] { '/' });
            string absolutePath = context.Server.MapPath(virtualPath);
            string directoryName = new DirectoryInfo(Path.GetDirectoryName(absolutePath).ToLower()).Name;

            //Firstly, determine the hostname of the site
            string hostName = context.Request.Url.Host;

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
                        context.Response.ContentType = "text/plain";
                        context.Response.WriteFile(absolutePath, false);
                        return;
                    }
                }
            }

            //If we get this far, it means there was no config entry for the current host name
            //In that case, we want to send a response indicating a robots file was found telling the search engine not to index anything

            //Serve the robots.txt file from the disk, it it exists
            context.Response.ContentType = "text/plain";
            context.Response.Write(c_RobotsNoIndex);
        }
    }
}