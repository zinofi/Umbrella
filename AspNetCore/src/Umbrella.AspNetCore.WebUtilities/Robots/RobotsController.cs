using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading;
using Umbrella.Utilities.Extensions;

namespace Umbrella.AspNetCore.WebUtilities.Robots
{
    [Route("robots.txt")]
    public class RobotsController : UmbrellaController
    {
        #region Private Constants
        private const string c_RobotsNoIndex = "User-agent: *\r\nDisallow: /";
        #endregion

        #region Private Members
        private readonly RobotsOptions m_RobotsConfig;
        private readonly IHostingEnvironment m_HostingEnvironment;
        #endregion

        #region Constructors
        public RobotsController(
            ILogger<RobotsController> logger,
            IOptions<RobotsOptions> robotsConfig,
            IHostingEnvironment hostingEnvironment)
            : base(logger)
        {
            m_RobotsConfig = robotsConfig.Value;
            m_HostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region Action Methods
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                //Get the host name excluding the port
                string hostName = Request.Host.Value.Split(':').First().ToLowerInvariant();

                //Try and find a mapping entry for the host name
                //Check there is a robots configuration - it may be that one is not configured
                //in which case the default string will always be returned.
                if (m_RobotsConfig != null && m_RobotsConfig.RobotMappings.TryGetValue(hostName, out string robotsFileName))
                {
                    string filePath = $@"{m_HostingEnvironment.WebRootPath}\{robotsFileName}";

                    if (System.IO.File.Exists(filePath))
                    {
                        string content = null;

                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            content = reader.ReadToEnd();
                        }

                        return Ok(content);
                    }
                }

                //If we get this far, we don't have a robots entry on disk
                //Render the default no index string
                return Ok(c_RobotsNoIndex);
            }
            catch(Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
        #endregion
    }
}