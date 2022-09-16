// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.TypeScript.Tools;

public class Program
{
	public static int Main(string[] args)
	{
		var app = new UmbrellaTypeScriptApp();

		if (args is null ||
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