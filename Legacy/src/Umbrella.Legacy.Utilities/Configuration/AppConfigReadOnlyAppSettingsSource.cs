using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Configuration;

namespace Umbrella.Legacy.Utilities.Configuration
{
    public class AppConfigReadOnlyAppSettingsSource : IReadOnlyAppSettingsSource
    {
        #region IReadOnlyAppSettingsAccessor Members
        public string GetValue(string key) => ConfigurationManager.AppSettings[key];
        #endregion
    }
}