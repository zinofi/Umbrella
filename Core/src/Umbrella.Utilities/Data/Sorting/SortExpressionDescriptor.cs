using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.Utilities.Data.Sorting
{
	/// <summary>
	/// Used to describe a sorting rule. One intended usage of this is for when an API controller
	/// receives data from a client about how data should be sorted and a JSON representation of this is
	/// deserialized to an instance of this type. The web projects contain model binders for performing custom
	/// JSON deserialization to this type.
	/// </summary>
	public class SortExpressionDescriptor : IDataExpressionDescriptor
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
		/// Gets or sets the <see cref="SortDirection"/>.
		/// </summary>
		public SortDirection Direction { get; set; }

		/// <summary>
		/// Returns true if the descriptor is valid.
		/// </summary>
		/// <returns>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </returns>
		public bool IsValid() => !string.IsNullOrEmpty(MemberName);
	}
}