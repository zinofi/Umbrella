namespace Umbrella.Utilities.Enumerations
{
	/// <summary>
	/// Used to specify the sort direction for a list of items. This exists to avoid using the SortDirection enum from the System.Web namespaces
    /// which isn't available in .NET Core.
	/// </summary>
	public enum SortDirection
	{
		Ascending = 0,
		Descending = 1
	}
}