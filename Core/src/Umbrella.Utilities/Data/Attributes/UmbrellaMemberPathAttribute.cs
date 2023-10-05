using CommunityToolkit.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Umbrella.Utilities.Data.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class UmbrellaMemberPathAttribute : Attribute
{
	[SetsRequiredMembers]
	public UmbrellaMemberPathAttribute(string value)
	{
		Guard.IsNotNullOrWhiteSpace(value);

		Value = value;
	}

	public required string Value { get; init; }
}