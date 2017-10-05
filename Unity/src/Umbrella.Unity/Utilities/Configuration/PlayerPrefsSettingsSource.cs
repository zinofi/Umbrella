using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities;
using Umbrella.Utilities.Configuration;
using UnityEngine;

namespace Umbrella.Unity.Utilities.Configuration
{
    public class PlayerPrefsSettingsSource : IAppSettingsSource
    {
        public string GetValue(string key)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            return PlayerPrefs.GetString(key);
        }

        public void SetValue(string key, string value)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            if (value != null)
                PlayerPrefs.SetString(key, value);
            else if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }
    }
}