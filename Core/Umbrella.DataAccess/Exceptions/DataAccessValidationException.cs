using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Exceptions
{
	public class DataAccessValidationException : ApplicationException
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