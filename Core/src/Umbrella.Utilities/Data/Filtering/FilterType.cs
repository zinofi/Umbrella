namespace Umbrella.Utilities.Data.Filtering;

/// <summary>
/// The type of the filter being applied.
/// All matches on strings are case-insensitive using the current culture.
/// </summary>
public enum FilterType
{
	/// <summary>
	/// Filters based on whether the target contains the source. Only valid for strings.
	/// </summary>
	Contains = 0,

	/// <summary>
	/// Filters from the start. Only valid for strings.
	/// </summary>
	StartsWith = 1,

	/// <summary>
	/// Filters from the end. Only valid for strings.
	/// </summary>
	EndsWith = 2,

	/// <summary>
	/// Checks for equality.
	/// </summary>
	Equal = 3,

	/// <summary>
	/// Check for inequality.
	/// </summary>
	NotEqual = 4,

	/// <summary>
	/// A greater than comparison.
	/// </summary>
	GreaterThan = 5,

	/// <summary>
	/// A greater than or equal comparison.
	/// </summary>
	GreaterThanOrEqual = 6,

	/// <summary>
	/// A less than comparison.
	/// </summary>
	LessThan = 7,

	/// <summary>
	/// A less than or equal comparison.
	/// </summary>
	LessThanOrEqual = 8
}