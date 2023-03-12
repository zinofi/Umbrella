namespace Umbrella.Utilities.Email.Abstractions;

/// <summary>
/// A generic class to aid in building emails based on HTML generated templates stored as static files on disk.
/// </summary>
public interface IEmailFactory
{
	/// <summary>
	/// Used to create a new email. Specify either an email template filename, or supply a raw html string to use instead in conjunction with the <paramref name="isRawHtml"/> parameter.
	/// </summary>
	/// <param name="source">The source template file or raw html to use.</param>
	/// <param name="isRawHtml">Indicates whether the source is a file or raw html.</param>
	/// <returns>The <see cref="EmailContent"/>.</returns>
	EmailContent CreateEmail(string source = "GenericTemplate", bool isRawHtml = false);
}