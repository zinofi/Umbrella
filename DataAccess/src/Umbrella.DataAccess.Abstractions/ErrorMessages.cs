namespace Umbrella.DataAccess.Abstractions;

/// <summary>
/// Common error messages used by EF repositories.
/// </summary>
public static class ErrorMessages
{
	/// <summary>
	/// The invalid property string length error message format
	/// </summary>
	public const string InvalidPropertyStringLengthErrorMessageFormat = "The {0} value must be between {1} and {2} characters in length.";

	/// <summary>
	/// The invalid property number range error message format
	/// </summary>
	public const string InvalidPropertyNumberRangeErrorMessageFormat = "The {0} value must be between {1} and {2}.";

	/// <summary>
	/// The bulk action concurrency exception error message
	/// </summary>
	public const string BulkActionConcurrencyExceptionErrorMessage = "A concurrency error has occurred whilst trying to update the items.";

	/// <summary>
	/// The concurrency exception error message format
	/// </summary>
	public const string ConcurrencyExceptionErrorMessageFormat = "A concurrency error has occurred whilst trying to save the item with id {0} or one of its dependants.";
}