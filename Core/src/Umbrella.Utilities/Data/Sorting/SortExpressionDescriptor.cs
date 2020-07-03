using System.Data.SqlTypes;
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
		/// Initializes a new instance of the <see cref="SortExpressionDescriptor"/> class.
		/// </summary>
		public SortExpressionDescriptor()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SortExpressionDescriptor"/> class.
		/// </summary>
		/// <param name="memberPath">The member path.</param>
		/// <param name="direction">The direction.</param>
		public SortExpressionDescriptor(string memberPath, SortDirection direction)
		{
			MemberPath = memberPath;
			Direction = direction;
		}

		/// <inheritdoc />
		public string MemberPath
		{
			get => _memberName;
			set => _memberName = value?.Trim();
		}

		/// <summary>
		/// Gets or sets the <see cref="SortDirection"/>.
		/// </summary>
		public SortDirection Direction { get; set; }

		/// <inheritdoc />
		public bool IsValid() => !string.IsNullOrEmpty(MemberPath);

		/// <inheritdoc />
		public override string ToString() => $"{MemberPath}:{Direction}";
	}
}