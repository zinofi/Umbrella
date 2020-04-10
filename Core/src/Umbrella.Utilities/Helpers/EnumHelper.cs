using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Helpers
{
	/// <summary>
	/// A static class containing helper methods for use with Enum types.
	/// </summary>
	public static class EnumHelper
    {
		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more
		/// enumerated constants to an equivalent enumerated object. A parameter specifies
		/// whether the operation is case-sensitive. The return value indicates whether the
		/// conversion succeeded.
		/// </summary>
		/// <param name="enumType">The enumeration type to which to convert the value.</param>
		/// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
		/// <param name="ignoreCase">true to ignore case; false to consider case.</param>
		/// <param name="result">
		/// When this method returns, result contains an object of type TEnum whose value
		/// is represented by value if the parse operation succeeds. If the parse operation
		/// fails, result contains the default value of the underlying type of TEnum. Note
		/// that this value need not be a member of the TEnum enumeration. This parameter
		/// is passed uninitialized.
		/// </param>
		/// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
		public static bool TryParseEnum(Type enumType, string value, bool ignoreCase, out object result)
		{
			result = null;

			try
			{
				result = Enum.Parse(enumType, value, ignoreCase);

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}