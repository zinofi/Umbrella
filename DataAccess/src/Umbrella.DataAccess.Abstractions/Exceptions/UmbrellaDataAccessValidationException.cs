using System;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	// TODO: V3 - Change validation behaviour to not throw exceptions. Use the SaveResult class so that validation errors can be aggregated
	// and returned to the user instead of 1 at a time using exceptions.
	public class UmbrellaDataAccessValidationException : UmbrellaDataAccessException
	{
		public DataValidationType ValidationType { get; internal set; }

		public UmbrellaDataAccessValidationException(string message, DataValidationType validationType = DataValidationType.Invalid)
			: base(message)
		{
			ValidationType = validationType;
		}

		public UmbrellaDataAccessValidationException(string message, Exception innerException, DataValidationType validationType = DataValidationType.Invalid)
			: base(message, innerException)
		{
			ValidationType = validationType;
		}
	}
}