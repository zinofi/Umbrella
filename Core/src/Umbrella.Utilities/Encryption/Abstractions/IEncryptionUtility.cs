namespace Umbrella.Utilities.Encryption.Abstractions;

/// <summary>
/// Defines the contract for a utility that can encrypt and decrypt string values.
/// </summary>
public interface IEncryptionUtility
{
	/// <summary>
	/// Decrypts the encrypted string.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The decrypted string.</returns>
	string DecryptString(string value);

	/// <summary>
	/// Encrypts the decrypted string.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The encrypted string.</returns>
	string EncryptString(string value);
}