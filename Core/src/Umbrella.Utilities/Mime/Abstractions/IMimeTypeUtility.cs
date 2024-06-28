namespace Umbrella.Utilities.Mime.Abstractions;

/// <summary>
/// A utility to lookup MIME Types.
/// </summary>
public interface IMimeTypeUtility
{
	/// <summary>
	/// Gets the file extension for the specified mime type. If the extension cannot be identified then null will be returned.
	/// </summary>
	/// <param name="mimeType">The mime type.</param>
	/// <returns>The file extension or null if the extension cannot be identified.</returns>
	string? GetFileExtension(string? mimeType);

	/// <summary>
	/// Gets the MIME Type of the specified filename or extension.
	/// If the extension cannot be identified then the default type of application/octet-stream will be returned.
	/// </summary>
	/// <param name="fileNameOrExtension">The file name or extension.</param>
	string GetMimeType(string fileNameOrExtension);
}