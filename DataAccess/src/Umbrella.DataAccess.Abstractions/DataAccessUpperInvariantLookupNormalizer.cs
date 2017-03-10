using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions.Interfaces;

namespace Umbrella.DataAccess.Abstractions
{
    public class DataAccessUpperInvariantLookupNormalizer : IDataAccessLookupNormalizer
    {
        public string Normalize(string value)
        {
            if (value == null)
                return null;

            return value.Normalize().ToUpperInvariant();
        }
    }
}