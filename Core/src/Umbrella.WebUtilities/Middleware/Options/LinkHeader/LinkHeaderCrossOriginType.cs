namespace Umbrella.WebUtilities.Middleware.Options.LinkHeader;

/// <summary>
/// The cross origin type.
/// </summary>
public enum LinkHeaderCrossOriginType
{
	/// <summary>
	/// No cross origin type.
	/// </summary>
	None,

	/// <summary>
	/// Anonymous cross origin type.
	/// </summary>
	Anonymous,

	/// <summary>
	/// Use credentials cross origin type.
	/// </summary>
	UseCredentials
}

/// <summary>
/// Extension methods for the <see cref="LinkHeaderCrossOriginType"/> enum type.
/// </summary>
public static class LinkHeaderCrossOriginTypeExtensions
{
	/// <summary>
	/// Converts to the enum value to its corresponding crossorigin attribute value.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The crossorigin attribute value.</returns>
	/// <exception cref="ArgumentException">Thrown when the <paramref name="value"/> has an unknown value.</exception>
	public static string ToCrossOriginString(this LinkHeaderCrossOriginType value) => value switch
	{
		LinkHeaderCrossOriginType.None => "none",
		LinkHeaderCrossOriginType.Anonymous => "anonymous",
		LinkHeaderCrossOriginType.UseCredentials => "use-credentials",
		_ => throw new ArgumentException($"The specified value: {value} is not supported.", nameof(value))
	};
}