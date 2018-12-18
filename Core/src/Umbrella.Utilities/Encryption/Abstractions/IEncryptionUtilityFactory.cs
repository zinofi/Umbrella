using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Encryption.Abstractions
{
    public interface IEncryptionUtilityFactory
    {
        T CreateEncryptionUtility<T>(string encryptionKey, string initializationVector) where T : IEncryptionUtility;
    }
}