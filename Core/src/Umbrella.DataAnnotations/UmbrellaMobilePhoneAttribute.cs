using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// A <see cref="RegularExpressionAttribute"/> used to validate a UK mobile phone number.
	/// </summary>
	/// <seealso cref="System.ComponentModel.DataAnnotations.RegularExpressionAttribute" />
	public class UmbrellaMobilePhoneAttribute : RegularExpressionAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaMobilePhoneAttribute"/> class.
		/// </summary>
		public UmbrellaMobilePhoneAttribute()
			: base(@"^07\d{3}\s?\d{6}$")
		{
		}
	}
}