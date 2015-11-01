using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Umbrella.Utilities.Configuration;

namespace Umbrella.WebUtilities.Robots
{
    public class RobotsConfig
    {
        private readonly Configuration m_Configuration;

        public RobotsConfig(Configuration config)
        {
            m_Configuration = config;
        }

        public RobotMappingsSection Settings
        {
			get
			{
                UmbrellaSectionGroup group = UmbrellaSectionGroup.GetSectionGroup(m_Configuration);
				if (group != null)
				{
					return group.GetConfigurationSection<RobotMappingsSection>("robotMappings");
				}

				return null;
			}
        }
    }
}