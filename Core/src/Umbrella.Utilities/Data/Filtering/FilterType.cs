namespace Umbrella.Utilities.Data.Filtering
{
	/// <summary>
	/// The type of the filter being applied. This is only valid for use when filtering on string data.
	/// All other data types will use exact matches.
	/// All matches on strings are case-insensitive using the current culture.
	/// </summary>
	public enum FilterType
	{
		/// <summary>
		/// Performs an exact match.
		/// </summary>
		Exact,

		/// <summary>
		/// Filters from the start
		/// </summary>
		StartsWith,

		/// <summary>
		/// Filters from the end
		/// </summary>
		EndsWith,

		/// <summary>
		/// Filters based on whether the target contains the source
		/// </summary>
		Contains
	}
}