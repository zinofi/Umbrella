using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using System.Reflection;
using System.Runtime.Loader;
using System.IO;
using Umbrella.Utilities;

namespace Umbrella.TypeScript.Tools
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new UmbrellaTypeScriptApp();

            if (args == null ||
                args.Length == 0 ||
                args[0].Equals("-?", StringComparison.OrdinalIgnoreCase) ||
                args[0].Equals("-h", StringComparison.OrdinalIgnoreCase) ||
                args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
            {
                app.ShowHelp();
                return 1;
            }

            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                app.WriteConsoleErrorMessage("Failed: " + ex.Message);
                return 1;
            }
        }
    }
}