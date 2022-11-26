using System.ComponentModel.DataAnnotations;
using Umbrella.DataAnnotations.RegularExpressions;

namespace Umbrella.DataAnnotations;

/// <summary>
/// A <see cref="RegularExpressionAttribute"/> used to validate a UK mobile phone number.
/// </summary>
/// <seealso cref="RegularExpressionAttribute" />
public class UmbrellaMobilePhoneAttribute : RegularExpressionAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaMobilePhoneAttribute"/> class.
	/// </summary>
	public UmbrellaMobilePhoneAttribute()
		: base(PhoneRegularExpressions.UKMobileRegexString)
	{
	}
}