namespace Umbrella.Utilities.DataAnnotations.Options;

/// <summary>
/// Options for use with the <see cref="ObjectGraphValidator" />.
/// </summary>
public class ObjectGraphValidatorOptions
{
	/// <summary>
	/// Gets or sets a delegate used to filter out properties which the validator should not try and recursively validate.
	/// Defaults to using a filter that excludes all value types that have an assembly name starting with "System".
	/// </summary>
	public Func<Type, bool> IgnorePropertyFilter { get; set; } = x => x.IsValueType && x.Assembly.FullName?.StartsWith("System", StringComparison.OrdinalIgnoreCase) is true;
}