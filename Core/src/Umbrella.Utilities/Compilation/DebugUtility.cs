using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Umbrella.AspNetCore.WebUtilities")]
[assembly: InternalsVisibleTo("Umbrella.AspNetCore.DynamicImage")]
namespace Umbrella.Utilities.Compilation
{
    /// <summary>
    /// This is an internal class used only for the purposes of debugging the library projects. Exposing this for use outside
    /// of these projects would be pointless once the libraries have been compiled in release mode.
    /// </summary>
    internal static class DebugUtility
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
