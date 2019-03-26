using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	// TODO: V3 - Change validation behaviour to not throw exceptions. Use the SaveResult class so that validation errors can be aggregated
	// and returned to the user instead of 1 at a time using exceptions.
	// TODO: V3 - Prefix name with Umbrella
	public class DataAccessValidationException : UmbrellaDataAccessException
	{
		public DataValidationType ValidationType { get; internal set; }

		public DataAccessValidationException(string message, DataValidationType validationType = DataValidationType.Invalid)
			: base(message)
		{
			ValidationType = validationType;
		}

		public DataAccessValidationException(string message, Exception innerException, DataValidationType validationType = DataValidationType.Invalid)
			: base(message, innerException)
		{
			ValidationType = validationType;
		}
	}
}