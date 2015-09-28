using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
using Umbrella.Utilities.Configuration.Exceptions;

namespace Umbrella.Utilities.Configuration
{
    public class UmbrellaSectionGroup : ConfigurationSectionGroup
    {
        #region Public Properties
        public T GetConfigurationSection<T>(string name) where T : class
        {
            return this.Sections[name] as T;
        }
        #endregion

        #region Public Static Methods
        public static UmbrellaSectionGroup GetSectionGroup(System.Configuration.Configuration config)
        {
            if (config == null)
                throw new ArgumentNullException($"The paramater {nameof(config)} cannot be null");

            return config.GetSectionGroup("umbrella") as UmbrellaSectionGroup;
        }
        #endregion
    }
}