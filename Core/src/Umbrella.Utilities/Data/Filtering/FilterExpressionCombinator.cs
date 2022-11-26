namespace Umbrella.Utilities.Data.Filtering;

/// <summary>
/// The combinator applied to multiple <see cref="FilterExpression{TItem}"/>.
/// </summary>
public enum FilterExpressionCombinator
{
	/// <summary>
	/// Uses OR logic
	/// </summary>
	Or,

	/// <summary>
	/// Uses AND logic
	/// </summary>
	And
}