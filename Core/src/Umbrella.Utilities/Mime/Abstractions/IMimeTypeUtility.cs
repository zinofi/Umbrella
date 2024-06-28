namespace Umbrella.Utilities.Mime.Abstractions;

/// <summary>
/// A utility to lookup MIME Types.
/// </summary>
public interface IMimeTypeUtility
{
	string? GetFileExtension(string mimeType);

	/// <summary>
	/// Gets the MIME Type of the specified filename or extension.
	/// If the extension cannot be identified then the default type of application/octet-stream will be returned.
	/// </summary>
	/// <param name="fileNameOrExtension">The file name or extension.</param>
	string GetMimeType(string fileNameOrExtension);
}