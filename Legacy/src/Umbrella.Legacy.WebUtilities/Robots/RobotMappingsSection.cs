using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;


namespace Umbrella.Legacy.WebUtilities.Robots
{
    public class RobotMappingsSection : ConfigurationSection
    {
        #region Public Properties
        [ConfigurationProperty("robots")]
        public RobotElementCollection Robots
        {
            get { return this["robots"] as RobotElementCollection; }
        }
        #endregion
    }
}