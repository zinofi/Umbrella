namespace Umbrella.WebUtilities.Security;

/// <summary>
/// Default web based security constants.
/// </summary>
public static class SecurityConstants
{
	/// <summary>
	/// The default nonce key used primarily by security middleware.
	/// </summary>
	public const string DefaultNonceKey = "UMBRELLA_WEB_NONCE";

	/// <summary>
	/// The default size of generated nonce values in bytes.
	/// </summary>
	public const int DefaultNonceBytes = 32;
}