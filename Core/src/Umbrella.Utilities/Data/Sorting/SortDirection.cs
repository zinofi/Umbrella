namespace Umbrella.Utilities.Data.Sorting
{
	/// <summary>
	/// Used to specify the sort direction for a list of items. This exists to avoid using the SortDirection enum from the System.Web namespaces
	/// which isn't available in .NET Core.
	/// </summary>
	public enum SortDirection
	{
		/// <summary>
		/// Sort in ascending order.
		/// </summary>
		Ascending = 0,

		/// <summary>
		/// Sort in descending order.
		/// </summary>
		Descending = 1
	}
}
// TODO: Remove this in favour of using the System.ComponentModel.ListSortDirection. Pointless to have duplication when the type is
// already there.