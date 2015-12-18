using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Compilation
{
    public static class DebugUtility
    {
        public static bool IsDebugMode
        {
            get
            {
                bool isDebugMode = false;

                WellWeAre(ref isDebugMode);

                return isDebugMode;
            }
        }

        [Conditional("DEBUG")]
        private static void WellWeAre(ref bool isDebugMode)
        {
            isDebugMode = true;
        }
    }
}
