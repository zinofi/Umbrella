using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.Utilities.Data.Filtering
{
	/// <summary>
	/// Used to describe a filtering rule. One intended usage of this is for when an API controller
	/// receives data from a client about how data should be filtered and a JSON representation of this is
	/// deserialized to an instance of this type. The web projects contain model binders for performing custom
	/// JSON deserialization to this type.
	/// </summary>
	public class FilterExpressionDescriptor : IExpressionDescriptor
	{
		private string _memberName;

		/// <summary>
		/// Gets or sets the name of the member.
		/// </summary>
		public string MemberName
		{
			get => _memberName;
			set => _memberName = value?.Trim();
		}

		/// <summary>
		/// Gets or sets the value used for filtering.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the filter type.
		/// </summary>
		public FilterType Type { get; set; }
	}
}