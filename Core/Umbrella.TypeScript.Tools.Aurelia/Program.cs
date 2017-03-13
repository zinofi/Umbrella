using System;

namespace Umbrella.TypeScript.Tools.Aurelia
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new UmbrellaTypeScriptAureliaApp();

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