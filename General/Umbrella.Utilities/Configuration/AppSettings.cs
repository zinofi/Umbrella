using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Configuration
{
    public abstract class AppSettings
    {
        public static T GetSetting<T>(T fallback = default(T), [CallerMemberName]string key = "", bool throwException = false)
        {
            string value = ConfigurationManager.AppSettings[key];

            if (!string.IsNullOrEmpty(value))
                return (T)Convert.ChangeType(value, typeof(T));
            else if (throwException)
                throw new ArgumentException(string.Format("The value for key: {0} is not valid. An appSetting with that key cannot be found", key));

            return typeof(T) == typeof(string) && fallback == null
                ? (T)Convert.ChangeType(string.Empty, typeof(T))
                : fallback;
        }

		public static T GetSettingEnum<T>(T fallback = default(T), [CallerMemberName]string key = "", bool throwException = false) where T : struct
		{
			string value = ConfigurationManager.AppSettings[key];

			T output;
			if (!string.IsNullOrEmpty(value) && typeof(Enum).IsAssignableFrom(typeof(T)) && Enum.TryParse<T>(value, true, out output))
				return output;
			else if (throwException)
				throw new ArgumentException(string.Format("The value for key: {0} is not valid. An appSetting with that key cannot be found", key));

			return fallback;
		}
    }
}