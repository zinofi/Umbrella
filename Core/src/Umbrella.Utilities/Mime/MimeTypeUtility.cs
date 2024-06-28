using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using MimeKit;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Mime.Abstractions;

namespace Umbrella.Utilities.Mime;

/// <summary>
/// A utility to lookup MIME Types. This type uses the MimeKit library internally.
/// </summary>
/// <seealso cref="IMimeTypeUtility" />
public class MimeTypeUtility : IMimeTypeUtility
{
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="MimeTypeUtility"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public MimeTypeUtility(ILogger<MimeTypeUtility> logger)
	{
		_logger = logger;
	}

	/// <summary>
	/// Gets the MIME Type of the specified filename or extension.
	/// If the extension cannot be identified then the default type of application/octet-stream will be returned.
	/// </summary>
	/// <param name="fileNameOrExtension">The file name or extension.</param>
	/// <exception cref="UmbrellaException">Thrown if there is a problem identifying the mime type.</exception>
	public string GetMimeType(string fileNameOrExtension)
	{
		Guard.IsNotNullOrWhiteSpace(fileNameOrExtension);

		try
		{
			return MimeTypes.GetMimeType(fileNameOrExtension);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { fileNameOrExtension }))
		{
			throw new UmbrellaException($"There was a problem identifying the mime type of the specified file name or extension: {fileNameOrExtension}", exc);
		}
	}

	/// <summary>
	/// Gets the file extension for the specified mime type. If the extension cannot be identified then null will be returned.
	/// </summary>
	/// <param name="mimeType">The mime type.</param>
	/// <returns>The file extension or null if the extension cannot be identified.</returns>
	/// <exception cref="UmbrellaException">Thrown if there is a problem identifying the file extension.</exception>
	public string? GetFileExtension(string mimeType)
	{
		Guard.IsNotNullOrWhiteSpace(mimeType);

		try
		{
			return MimeTypes.TryGetExtension(mimeType, out string? extension) ? extension : null;
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { mimeType }))
		{
			throw new UmbrellaException($"There was a problem identifying the file extension of the specified mime type: {mimeType}", exc);
		}
	}
}