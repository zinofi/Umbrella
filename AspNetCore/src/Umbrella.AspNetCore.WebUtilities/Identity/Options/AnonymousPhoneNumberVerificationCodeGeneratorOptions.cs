using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Identity.Options;

/// <summary>
/// Options for use with the <see cref="Identity" /> type. 
/// </summary>
/// <seealso cref="ISanitizableUmbrellaOptions" />
/// <seealso cref="IValidatableUmbrellaOptions" />
public class AnonymousPhoneNumberVerificationCodeGeneratorOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// 
	/// </summary>
	public string AnonymousSecurityStamp { get; set; } = "K32ZY5PDKWHMHCA5D3NXB5UJUMZVCO6N";

	/// <inheritdoc />
	public void Sanitize()
	{
		AnonymousSecurityStamp = AnonymousSecurityStamp.Trim();
	}

	/// <inheritdoc />
	public void Validate()
	{
		Guard.IsNotNullOrWhiteSpace(AnonymousSecurityStamp);
	}
}