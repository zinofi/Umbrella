using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Umbrella.Utilities.Encryption.Interfaces;

namespace Umbrella.Utilities.Encryption
{
    /// <summary>
    /// Utility to provide string encryption capabilities
    /// </summary>
    public class TripleDESEncryptionUtility : EncryptionUtility<TripleDESCryptoServiceProvider>
    {
        #region Constructors
        /// <summary>
        /// Constructor with values for the Key and IV.
        /// </summary>
        /// <param name="encryptionKey">The encryption key to use</param>
        /// <param name="initializationVector">The initialization vector to use</param>
		public TripleDESEncryptionUtility(ILogger logger)
			: base(logger)
		{
		}
        #endregion
    }
}