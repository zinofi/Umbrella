using System.Security.Cryptography;

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
		public TripleDESEncryptionUtility(string encryptionKey, string initializationVector)
			: base(encryptionKey, initializationVector)
		{
		}
        #endregion
    }
}