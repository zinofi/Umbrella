namespace Umbrella.Extensions.Logging.Azure.Management;

/// <summary>
/// The result of an operation to delete a log table.
/// </summary>
public enum AzureTableStorageLogDeleteOperationResult
{
	/// <summary>
	/// Indicates that the operation was a success.
	/// </summary>
	Success,

	/// <summary>
	/// Indicates that the target table could not be deleted as it it does not exist.
	/// </summary>
	NotFound
}