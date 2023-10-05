namespace Umbrella.Utilities.Helpers;

/// <summary>
/// A static class used to normalize file paths for the current platform.
/// </summary>
public static class PathHelper
{
	/// <summary>
	/// Normalizes the specified <paramref name="path"/>.
	/// </summary>
	/// <param name="path">The path.</param>
	/// <returns>The normalized path.</returns>
	public static string PlatformNormalize(string path) => string.IsNullOrWhiteSpace(path) ? path : path.Replace('\\', Path.DirectorySeparatorChar);
}