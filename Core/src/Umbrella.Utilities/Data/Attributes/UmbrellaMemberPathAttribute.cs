using CommunityToolkit.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Umbrella.Utilities.Data.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class UmbrellaMemberPathAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaMemberPathAttribute"/> class.
	/// </summary>
	/// <param name="value">The value.</param>
	[SetsRequiredMembers]
	public UmbrellaMemberPathAttribute(string value)
	{
		Guard.IsNotNullOrWhiteSpace(value);

		Value = value;
	}

	/// <summary>
	/// Gets the value.
	/// </summary>
	public required string Value { get; init; }
}