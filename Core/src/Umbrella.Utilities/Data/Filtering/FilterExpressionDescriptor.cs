using System;
using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.Utilities.Data.Filtering
{
	/// <summary>
	/// Used to describe a filtering rule. One intended usage of this is for when an API controller
	/// receives data from a client about how data should be filtered and a JSON representation of this is
	/// deserialized to an instance of this type. The web projects contain model binders for performing custom
	/// JSON deserialization to this type.
	/// </summary>
	[Serializable]
	public class FilterExpressionDescriptor : IDataExpressionDescriptor
	{
		private string? _memberPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterExpressionDescriptor"/> class.
		/// </summary>
		public FilterExpressionDescriptor()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterExpressionDescriptor"/> class.
		/// </summary>
		/// <param name="memberPath">The member path.</param>
		/// <param name="value">The value.</param>
		/// <param name="type">The type.</param>
		public FilterExpressionDescriptor(string memberPath, string? value, FilterType type)
		{
			MemberPath = memberPath;
			Value = value;
			Type = type;
		}

		/// <summary>
		/// Gets or sets the path of the member.
		/// </summary>
		public string? MemberPath
		{
			get => _memberPath;
			set => _memberPath = value?.Trim();
		}

		/// <summary>
		/// Gets or sets the value used for filtering.
		/// </summary>
		public string? Value { get; set; }

		/// <summary>
		/// Gets or sets the filter type.
		/// </summary>
		public FilterType Type { get; set; }

		/// <inheritdoc />
		/// <remarks>This will return true even if the <see cref="Value"/> is null as we might need to filter on that, e.g. looking for matches that have no value.</remarks>
		public bool IsValid() => !string.IsNullOrEmpty(MemberPath);

		/// <inheritdoc />
		public override string ToString() => $"{MemberPath}:{Value}:{Type}";
	}
}