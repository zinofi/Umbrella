using Umbrella.DataAccess.Exceptions.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Exceptions
{
	public class DataValidationException : ApplicationException
	{
		public DataValidationType ValidationType { get; internal set; }

		public DataValidationException(string message, DataValidationType validationType = DataValidationType.Invalid)
			: base(message)
		{
			ValidationType = validationType;
		}

		public DataValidationException(string message, Exception innerException, DataValidationType validationType = DataValidationType.Invalid)
			: base(message, innerException)
		{
			ValidationType = validationType;
		}
	}
}