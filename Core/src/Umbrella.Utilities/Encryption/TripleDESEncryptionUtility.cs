using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Umbrella.Utilities.Encryption
{
	/// <summary>
	/// Utility to provide string encryption capabilities
	/// </summary>
	public class TripleDESEncryptionUtility : EncryptionUtility<TripleDESCryptoServiceProvider>
    {
		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="TripleDESEncryptionUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public TripleDESEncryptionUtility(ILogger<TripleDESEncryptionUtility> logger)
			: base(logger)
		{
		}
        #endregion
    }
}