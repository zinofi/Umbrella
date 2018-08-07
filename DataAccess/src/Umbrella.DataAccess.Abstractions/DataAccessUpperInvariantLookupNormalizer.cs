using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions.Interfaces;

namespace Umbrella.DataAccess.Abstractions
{
    public class DataAccessUpperInvariantLookupNormalizer : IDataAccessLookupNormalizer
    {
        #region Private Members
        private readonly ILogger m_Log;
        #endregion

        #region Constructors
        public DataAccessUpperInvariantLookupNormalizer(ILogger<DataAccessUpperInvariantLookupNormalizer> logger)
            => m_Log = logger;
        #endregion

        #region IDataAccessLookupNormalizer Members
        public string Normalize(string value)
        {
            try
            {
                return value?.Normalize()?.ToUpperInvariant();
            }
            catch (Exception exc) when (m_Log.WriteError(exc, message: "String normalization to form C failed. The source string has not been logged for security reasons."))
            {
                throw;
            }
        } 
        #endregion
    }
}