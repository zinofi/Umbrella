namespace Umbrella.Utilities.Sorting
{
	/// <summary>
	/// Extension methods for the <see cref="SortDirection"/> enum.
	/// </summary>
	public static class SortDirectionExtensions
	{
		/// <summary>
		/// Converts the provided <see cref="SortDirection"/> to the corresponding SQL sort direction, i.e. ASC or DESC
		/// </summary>
		/// <param name="sortDirection">The sort direction.</param>
		/// <returns>A string of the corresponding SQL sort direction.</returns>
		public static string ToSqlSortDirection(this SortDirection sortDirection) => sortDirection == SortDirection.Ascending ? "ASC" : "DESC";
	}
}