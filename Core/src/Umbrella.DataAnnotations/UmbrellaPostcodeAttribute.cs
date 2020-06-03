using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// Specifies that a data field must match the regular expression for a UK Postcode.
	/// </summary>
	/// <seealso cref="System.ComponentModel.DataAnnotations.RegularExpressionAttribute" />
	public class UmbrellaPostcodeAttribute : RegularExpressionAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaPostcodeAttribute"/> class.
		/// </summary>
		public UmbrellaPostcodeAttribute()
			: base("^((([A-Pa-pR-UWYZr-uwyz](\\d([A-HJKSTUWa-hjkstuw]|\\d)?|" +
										  "[A-Ha-hK-Yk-y]\\d([AaBbEeHhMmNnPpRrVvWwXxYy]|\\d)?))\\s*" +
										  "(\\d[ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2})?)|[Gg][Ii][Rr]\\s*0[Aa][Aa])$")
		{
		}
	}
}