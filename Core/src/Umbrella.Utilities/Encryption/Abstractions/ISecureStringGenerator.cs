using System.Security.Cryptography;

namespace Umbrella.Utilities.Encryption.Abstractions
{
	/// <summary>
	/// A utility to generate random strings using the default cryptographically strong <see cref="RandomNumberGenerator"/> class internally.
	/// </summary>
	public interface ISecureStringGenerator
    {
		/// <summary>
		/// Generates a random string of the specified length using the specified options.
		/// If the sum of <paramref name="numbers"/>, <paramref name="upperCaseCharacters"/> and <paramref name="specialCharacters"/>
		/// is less than the <paramref name="length"/>, the remaining characters of the generated string will be filled using lowercase characters.
		/// </summary>
		/// <param name="length">The length.</param>
		/// <param name="numbers">The number of numbers.</param>
		/// <param name="upperCaseCharacters">The number of upper case letters.</param>
		/// <param name="specialCharacters">The number of special characters.</param>
		/// <returns>The generated string.</returns>
		string Generate(int length = 8, int numbers = 0, int upperCaseCharacters = 0, int specialCharacters = 0);
    }
}