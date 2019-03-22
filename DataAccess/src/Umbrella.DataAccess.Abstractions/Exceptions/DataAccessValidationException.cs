using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
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