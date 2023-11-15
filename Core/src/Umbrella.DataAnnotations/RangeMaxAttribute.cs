namespace Umbrella.DataAnnotations;

/// <summary>
/// Extends the <see cref="RangeAttribute" /> using either <see cref="double.MinValue" /> or <see cref="int.MinValue" /> as the default for convenience.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RangeMaxAttribute : RangeAttribute
{
	/// <summary>
	/// Create a new instance.
	/// </summary>
	/// <param name="maximum">The maximum.</param>
	public RangeMaxAttribute(double maximum)
		: base(double.MinValue, maximum)
	{
	}

	/// <summary>
	/// Create a new instance.
	/// </summary>
	/// <param name="maximum">The maximum.</param>
	public RangeMaxAttribute(int maximum)
		: base(int.MinValue, maximum)
	{
	}
}