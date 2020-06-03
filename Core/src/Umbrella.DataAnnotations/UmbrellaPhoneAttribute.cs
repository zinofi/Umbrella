using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// Specifies that a data field must match the regular expression for a UK Phone Number.
	/// </summary>
	/// <seealso cref="System.ComponentModel.DataAnnotations.RegularExpressionAttribute" />
	public class UmbrellaPhoneAttribute : RegularExpressionAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaPhoneAttribute"/> class.
		/// </summary>
		public UmbrellaPhoneAttribute()
			: base(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$")
		{
		}
	}
}