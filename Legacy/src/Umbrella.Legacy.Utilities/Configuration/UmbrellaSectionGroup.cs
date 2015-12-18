using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;

namespace Umbrella.Legacy.Utilities.Configuration
{
    public class UmbrellaSectionGroup : ConfigurationSectionGroup
    {
        #region Public Methods
        public T GetConfigurationSection<T>(string name) where T : class
        {
            return Sections[name] as T;
        }
        #endregion

        #region Public Static Methods
        public static UmbrellaSectionGroup GetSectionGroup(System.Configuration.Configuration config)
        {
            if (config == null)
                throw new ArgumentNullException($"The parameter {nameof(config)} cannot be null");

            return config.GetSectionGroup("umbrella") as UmbrellaSectionGroup;
        }
        #endregion
    }
}