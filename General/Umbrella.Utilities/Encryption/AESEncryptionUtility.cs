using System.Security.Cryptography;

namespace Umbrella.Utilities.Encryption
{
    /// <summary>
    /// Utility to provide string encryption capabilities
    /// </summary>
    public class AesEncryptionUtility : EncryptionUtility<AesManaged>
	{
		#region Constructors
        /// <summary>
        /// Constructor with values for the Key and IV.
        /// </summary>
        /// <param name="encryptionKey">The encryption key to use</param>
        /// <param name="initializationVector">The initialization vector to use</param>
		public AesEncryptionUtility(string encryptionKey, string initializationVector)
			: base(encryptionKey, initializationVector)
		{
		}
        #endregion
	}
}