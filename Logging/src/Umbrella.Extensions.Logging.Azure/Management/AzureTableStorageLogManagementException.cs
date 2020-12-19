using System;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Extensions.Logging.Azure.Management
{
	/// <summary>
	/// Represents and exception thrown by the Azure Table Storage Log Management code.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Exceptions.UmbrellaException" />
	public class AzureTableStorageLogManagementException : UmbrellaException
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="AzureTableStorageLogManagementException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public AzureTableStorageLogManagementException(string message)
            : base(message)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="AzureTableStorageLogManagementException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public AzureTableStorageLogManagementException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}