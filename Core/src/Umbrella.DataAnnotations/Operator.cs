namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// Specifies the type of comparision operation to perform during contingent validation operations.
	/// </summary>
	public enum Operator
	{
		/// <summary>
		/// Checks for equality.
		/// </summary>
		EqualTo,

		/// <summary>
		/// Checks for inequality.
		/// </summary>
		NotEqualTo,

		/// <summary>
		/// Checks that the property has a value greater than the named dependent property.
		/// </summary>
		GreaterThan,

		/// <summary>
		/// Checks that the property has a value less than the named dependent property.
		/// </summary>
		LessThan,

		/// <summary>
		/// Checks that the property has a value greater than or equal to the named dependent property.
		/// </summary>
		GreaterThanOrEqualTo,

		/// <summary>
		/// Checks that the property has a value less than or equal to the named dependent property.
		/// </summary>
		LessThanOrEqualTo,

		/// <summary>
		/// Checks for equality using the specified regular expression.
		/// </summary>
		RegExMatch,

		/// <summary>
		/// Checks for inequality using the specified regular expression.
		/// </summary>
		NotRegExMatch
	}
}