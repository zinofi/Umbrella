namespace Umbrella.Utilities.Data.Sorting
{
	/// <summary>
	/// Used to describe a sorting rule. One intended usage of this is for when an API controller
	/// receives data from a client about how data should be sorted and a JSON representation of this is
	/// deserialized to an instance of this type. The web projects contain model binders for performing custom
	/// JSON deserialization to this type.
	/// </summary>
	public class SortExpressionDescriptor
	{
		/// <summary>
		/// Gets or sets the name of the member.
		/// </summary>
		/// <value>
		/// The name of the member.
		/// </value>
		public string MemberName { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="SortDirection"/>.
		/// </summary>
		/// <value>
		/// The <see cref="SortDirection"/>.
		/// </value>
		public SortDirection Direction { get; set; }
	}
}