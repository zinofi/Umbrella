using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Umbrella.AspNetCore.WebUtilities")]
[assembly: InternalsVisibleTo("Umbrella.AspNetCore.DynamicImage")]
[assembly: InternalsVisibleTo("Umbrella.DynamicImage.Caching.AzureStorage.Test")]
[assembly: InternalsVisibleTo("Umbrella.DynamicImage.Impl.Test")]
[assembly: InternalsVisibleTo("Umbrella.DynamicImage.Test")]
[assembly: InternalsVisibleTo("Umbrella.FileSystem.Test")]
namespace Umbrella.Utilities.Compilation
{
    /// <summary>
    /// This is an internal class used only for the purposes of debugging the library projects. Exposing this for use outside
    /// of these projects would be pointless once the libraries have been compiled in release mode.
    /// </summary>
    internal static class DebugUtility
    {
        public static bool IsDebug
        {
            get
            {
                bool isDebugMode = false;

                IAmDebug(ref isDebugMode);

                return isDebugMode;
            }
        }

        public static bool IsTeamServices
        {
            get
            {
                bool isTeamServices = false;

                IAmTeamServices(ref isTeamServices);

                return isTeamServices;
            }
        }

        public static string BuildConfiguration
        {
            get
            {
                string configuration = "release";

#if TEAMSERVICES
                configuration = "teamservices";
#elif DEBUG
                configuration = "debug";
#endif

                return configuration;
            }
        }

        [Conditional("DEBUG")]
        private static void IAmDebug(ref bool isDebugMode)
        {
            isDebugMode = true;
        }

        [Conditional("TEAMSERVICES")]
        private static void IAmTeamServices(ref bool isTeamServices)
        {
            isTeamServices = true;
        }
    }
}