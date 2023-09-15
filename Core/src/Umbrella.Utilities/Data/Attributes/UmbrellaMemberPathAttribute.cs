using CommunityToolkit.Diagnostics;

namespace Umbrella.Utilities.Data.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class UmbrellaMemberPathAttribute : Attribute
{
	public UmbrellaMemberPathAttribute(string value)
	{
		Guard.IsNotNullOrWhiteSpace(value);

		Value = value;
	}

	public required string Value { get; set; }
}