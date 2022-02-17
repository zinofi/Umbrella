using System.ComponentModel.DataAnnotations;
using Umbrella.DataAnnotations.RegularExpressions;

namespace Temp365.Dental.Shared.EntityConstants.Attributes
{
	/// <summary>
	/// Specifies that a data field must be a valid URL.
	/// </summary>
	/// <seealso cref="System.ComponentModel.DataAnnotations.RegularExpressionAttribute" />
	public class UmbrellaUrlAttribute : RegularExpressionAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaUrlAttribute"/> class.
		/// </summary>
		public UmbrellaUrlAttribute()
			: base(UrlRegularExpressions.UrlRegexString)
		{
		}
	}
}