using System;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.Abstractions
{
	public class DataAccessUpperInvariantLookupNormalizer : IDataAccessLookupNormalizer
	{
		#region Private Members
		private readonly ILogger _log;
		#endregion

		#region Constructors
		public DataAccessUpperInvariantLookupNormalizer(ILogger<DataAccessUpperInvariantLookupNormalizer> logger)
		{
			_log = logger;
		}
		#endregion

		#region IDataAccessLookupNormalizer Members
		public string Normalize(string value, bool trim = true)
		{
			if (value is null)
				return null;

			try
			{
				return trim ? value.Normalize().TrimToUpperInvariant() : value.Normalize().ToUpperInvariant();
			}
			catch (Exception exc) when (_log.WriteError(exc, new { value = "Value not logged for security reasons.", trim }, "String normalization to form C failed. The source string has not been logged for security reasons.", returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem normalizing the specified value.", exc);
			}
		}
		#endregion
	}
}