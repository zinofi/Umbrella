namespace Umbrella.Utilities.Data.Abstractions;

/// <summary>
/// An abstraction representing a data expression descriptor.
/// </summary>
public interface IDataExpressionDescriptor
{
	/// <summary>
	/// Gets or sets the name of the member.
	/// </summary>
	string? MemberPath { get; set; }

	/// <summary>
	/// Returns true if the descriptor is valid.
	/// </summary>
	/// <returns>
	///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
	/// </returns>
	bool IsValid();
}