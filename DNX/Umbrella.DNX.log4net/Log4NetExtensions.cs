using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using log4net;
using log4net.Config;

namespace Umbrella.DNX.log4net
{
    public static class Log4NetExtensions
    {
        public static void ConfigureLog4Net(this IApplicationEnvironment appEnv, string configFileRelativePath)
        {
            GlobalContext.Properties["appRoot"] = appEnv.ApplicationBasePath;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(appEnv.ApplicationBasePath, configFileRelativePath)));
        }
    }
}
