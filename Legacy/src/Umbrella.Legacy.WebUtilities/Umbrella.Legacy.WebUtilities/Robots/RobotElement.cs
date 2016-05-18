using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Umbrella.Legacy.WebUtilities.Robots
{
    public class RobotElement : ConfigurationElement
    {
        #region Public Properties
        [ConfigurationProperty("hostName", IsRequired = true, IsKey = true)]
        public string HostName
        {
            get { return this["hostName"].ToString(); }
        }

        [ConfigurationProperty("fileName")]
        public string FileName
        {
            get { return this["fileName"].ToString(); }
        }
        #endregion
    }
}