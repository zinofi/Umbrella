namespace Umbrella.Utilities.Data.Abstractions;

/// <summary>
/// A utility used to normalize string values.
/// </summary>
public interface IDataLookupNormalizer
{
	/// <summary>
	/// Normalizes the specified value.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="trim">if set to <c>true</c> the value will be trimmed of leading and trailing whitespace before being normalized.</param>
	/// <returns>The normalized value.</returns>
	string Normalize(string value, bool trim = true);
}