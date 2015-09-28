using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using log4net;

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