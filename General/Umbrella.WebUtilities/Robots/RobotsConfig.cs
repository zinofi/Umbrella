using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbrella.Utilities.Configuration;

namespace Umbrella.WebUtilities.Robots
{
    public class RobotsConfig
    {
        public static RobotMappingsSection Settings
        {
			get
			{
                UmbrellaSectionGroup group = null; //TODO: UmbrellaSectionGroup.GetSectionGroup("");
				if (group != null)
				{
					return group.GetConfigurationSection<RobotMappingsSection>("robotMappings");
				}

				return null;
			}
        }
    }
}