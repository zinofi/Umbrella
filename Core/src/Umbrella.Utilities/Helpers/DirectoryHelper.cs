namespace Umbrella.Utilities.Helpers;

/// <summary>
/// Contains helper methods for working with directories.
/// </summary>
public static class DirectoryHelper
{
	/// <summary>
	/// Copies the specified directory to the target directory.
	/// </summary>
	/// <param name="sourceDir">The source directory.</param>
	/// <param name="targetDir">The target directory.</param>
	/// <param name="recursive">Whether to copy recursively.</param>
	/// <param name="overwrite">Whether to overwrite existing files.</param>
	private static void Copy(string sourceDir, string targetDir, bool recursive, bool overwrite)
	{
		Directory.CreateDirectory(targetDir);

		foreach (string file in Directory.GetFiles(sourceDir))
		{
			string fileName = Path.GetFileName(file);
			string destFile = Path.Combine(targetDir, fileName);

			File.Copy(file, destFile, overwrite);
		}

		foreach (string subDir in Directory.GetDirectories(sourceDir))
		{
			string dirName = Path.GetFileName(subDir);
			string destDir = Path.Combine(targetDir, dirName);

			if (recursive)
				Copy(subDir, destDir, true, overwrite);
		}
	}
}