namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// Specifies the type of comparision operation to perform during contingent validation operations.
	/// </summary>
	public enum Operator
	{
		EqualTo,
		NotEqualTo,
		GreaterThan,
		LessThan,
		GreaterThanOrEqualTo,
		LessThanOrEqualTo,
		RegExMatch,
		NotRegExMatch
	}
}