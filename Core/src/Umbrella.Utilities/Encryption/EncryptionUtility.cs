using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption.Abstractions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Encryption
{
	public abstract class EncryptionUtility<T> : IEncryptionUtility
		where T : SymmetricAlgorithm, new()
	{
        #region Protected Properties
        protected ILogger Log { get; }
        protected SymmetricAlgorithm Algorithm { get; private set; }
        protected ICryptoTransform Encryptor { get; private set; }
        protected ICryptoTransform Decryptor { get; private set; }
        #endregion

        #region Constructors
        public EncryptionUtility(ILogger logger)
        {
            Log = logger;   
        }
        #endregion

        #region Internal Methods
        internal byte[] Encrypt(string value)
        {
            try
            {
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, Encryptor, CryptoStreamMode.Write))
                    {
                        byte[] converted = ConvertStringToByteArray(value);

                        csEncrypt.Write(converted, 0, converted.Length);
                        csEncrypt.FlushFinalBlock();

                        return msEncrypt.ToArray();
                    }
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, value))
            {
                throw;
            }
        }

        internal string Decrypt(byte[] value)
        {
            try
            {
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, Decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(value, 0, value.Length);
                        csDecrypt.Close();

                        return ConvertByteArrayToString(msDecrypt.ToArray());
                    }
                }
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the encryption utility with values for the Key and IV.
        /// </summary>
        /// <param name="encryptionKey">The encryption key to use</param>
        /// <param name="initializationVector">The initialization vector to use</param>
        public void Initialize(string encryptionKey, string initializationVector)
        {
            try
            {
                // Create provider
                Algorithm = new T()
                {
                    // Assign key and iv
                    Mode = CipherMode.CBC,
                    Key = ConvertStringToByteArray(encryptionKey),
                    IV = ConvertStringToByteArray(initializationVector)
                };

                byte[] key = Algorithm.Key;
                byte[] iV = Algorithm.IV;

                // Setup encryptor and decryptor
                Encryptor = Algorithm.CreateEncryptor(key, iV);
                Decryptor = Algorithm.CreateDecryptor(key, iV);
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        /// <summary>
        /// Method to encrypt a given string.
        /// </summary>
        /// <param name="value">The string to encrypt.</param>
        /// <returns>The encryption result.</returns>
        public string EncryptString(string value)
        {
            try
            {
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, Encryptor, CryptoStreamMode.Write))
                    {
                        byte[] converted = Encoding.UTF8.GetBytes(value);

                        csEncrypt.Write(converted, 0, converted.Length);
                        csEncrypt.FlushFinalBlock();

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, value))
            {
                throw;
            }
        }

        /// <summary>
        /// Method to decrypt a given byte array.
        /// </summary>
        /// <param name="value">The byte array to decrypt.</param>
        /// <returns>The decryption result.</returns>
        public string DecryptString(string value)
        {
            try
            {
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, Decryptor, CryptoStreamMode.Write))
                    {
                        byte[] converted = Convert.FromBase64String(value);

                        csDecrypt.Write(converted, 0, converted.Length);
                        csDecrypt.Close();

                        return Encoding.UTF8.GetString(msDecrypt.ToArray());
                    }
                }
            }
            catch (Exception exc) when (Log.WriteError(exc, value))
            {
                throw;
            }
        }

        /// <summary>
        /// Disposes of the current instance
        /// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes of the resources used by this instance.
		/// </summary>
		/// <param name="disposing">A flag used to determine if this method has been called from the IDisposable.Dispose method.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Decryptor != null)
				{
					Decryptor.Dispose();
					Decryptor = null;
				}

				if (Encryptor != null)
				{
					Encryptor.Dispose();
					Encryptor = null;
				}

				if (Algorithm != null)
				{
					Algorithm.Dispose();
					Algorithm = null;
				}
			}
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to convert a string to a unicode byte array.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>The converted byte array.</returns>
        private byte[] ConvertStringToByteArray(string str) => Encoding.Default.GetBytes(str);

        /// <summary>
        /// Method to convert a byte array to a unicode string.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>The converted string.</returns>
        private string ConvertByteArrayToString(byte[] bytes) => Encoding.Default.GetString(bytes);

        #endregion
	}
}