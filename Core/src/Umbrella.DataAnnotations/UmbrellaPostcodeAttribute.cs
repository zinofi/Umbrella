using System.ComponentModel.DataAnnotations;
using Umbrella.DataAnnotations.RegularExpressions;

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
			: base(PostcodeRegularExpressions.UKPostcodeRegexString)
		{
		}
	}
}