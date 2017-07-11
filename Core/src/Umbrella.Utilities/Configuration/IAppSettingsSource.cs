using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Configuration
{
    public interface IAppSettingsSource : IReadOnlyAppSettingsSource
    {
        void SetValue(string key, string value);
    }
}