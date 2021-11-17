using System.Threading.Tasks;

namespace Umbrella.AspNetCore.WebUtilities.Identity.Abstractions
{
	/// <summary>
	/// A utility used to generate codes which can be used to verify the phone numbers of anonymous users.
	/// </summary>
	public interface IAnonymousPhoneNumberVerificationCodeGenerator
	{
		/// <summary>
		/// Create a verification code for the specified <paramref name="phoneNumber"/>.
		/// </summary>
		/// <param name="phoneNumber">The phone number.</param>
		/// <returns>The verification code.</returns>
		Task<string> CreateAsync(string phoneNumber);

		/// <summary>
		/// Verifies the specified verification <paramref name="code"/> is valid for the specified <paramref name="phoneNumber"/>.
		/// </summary>
		/// <param name="phoneNumber">The phone number.</param>
		/// <param name="code">The code.</param>
		/// <returns><see langword="true"/> if the code is valid; otherwise <see langword="false" />.</returns>
		Task<bool> VerifyAsync(string phoneNumber, string code);
	}
}