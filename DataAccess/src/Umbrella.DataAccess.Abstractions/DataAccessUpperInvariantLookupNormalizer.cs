using Microsoft.Extensions.Logging;
using System;
using Umbrella.DataAccess.Abstractions.Interfaces;

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
			if (value == null)
				return null;

            try
            {
				if (trim)
					value = value.Trim();

                return value.Normalize().ToUpperInvariant();
            }
            catch (Exception exc) when (_log.WriteError(exc, message: "String normalization to form C failed. The source string has not been logged for security reasons."))
            {
                throw;
            }
        } 
        #endregion
    }
}