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
		/// Filters based on whether the target contains the source
		/// </summary>
		Contains = 0,

		/// <summary>
		/// Filters from the start
		/// </summary>
		StartsWith = 1,

		/// <summary>
		/// Filters from the end
		/// </summary>
		EndsWith = 2,

		/// <summary>
		/// Performs an exact match.
		/// </summary>
		Exact = 3
	}
}