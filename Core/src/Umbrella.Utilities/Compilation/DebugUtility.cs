using System.Diagnostics;

namespace Umbrella.Utilities.Compilation;

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

	public static bool IsAzureDevOps
	{
		get
		{
			bool isAzureDevOps = false;

			IAmAzureDevOps(ref isAzureDevOps);

			return isAzureDevOps;
		}
	}

	public static string BuildConfiguration
	{
		get
		{
			string configuration = "release";

#if AZUREDEVOPS
                configuration = "azuredevops";
#elif DEBUG
			configuration = "debug";
#endif

			return configuration;
		}
	}

	[Conditional("DEBUG")]
	private static void IAmDebug(ref bool isDebugMode) => isDebugMode = true;

	[Conditional("AZUREDEVOPS")]
	private static void IAmAzureDevOps(ref bool isAzureDevOps) => isAzureDevOps = true;
}