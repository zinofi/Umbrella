using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption.Interfaces;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Encryption
{
	public abstract class EncryptionUtility<T> : IEncryptionUtility, IDisposable
		where T : SymmetricAlgorithm, new()
	{
		#region Protected Members
		protected SymmetricAlgorithm p_Algorithm;
		protected ICryptoTransform p_Encryptor;
		protected ICryptoTransform p_Decryptor;
        protected readonly ILogger m_Logger;
		#endregion

		#region Constructors
        public EncryptionUtility(ILogger logger)
        {
            m_Logger = logger;   
        }
        #endregion

        #region Internal Methods
        internal byte[] Encrypt(string value)
        {
            try
            {
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, p_Encryptor, CryptoStreamMode.Write))
                    {
                        byte[] converted = ConvertStringToByteArray(value);

                        csEncrypt.Write(converted, 0, converted.Length);
                        csEncrypt.FlushFinalBlock();

                        return msEncrypt.ToArray();
                    }
                }
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, value))
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
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, p_Decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(value, 0, value.Length);
                        csDecrypt.Close();

                        return ConvertByteArrayToString(msDecrypt.ToArray());
                    }
                }
            }
            catch (Exception exc) when (m_Logger.WriteError(exc))
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
                p_Algorithm = new T();

                // Assign key and iv
                p_Algorithm.Mode = CipherMode.CBC;
                p_Algorithm.Key = ConvertStringToByteArray(encryptionKey);
                p_Algorithm.IV = ConvertStringToByteArray(initializationVector);

                byte[] key = p_Algorithm.Key;
                byte[] iV = p_Algorithm.IV;

                // Setup encryptor and decryptor
                p_Encryptor = p_Algorithm.CreateEncryptor(key, iV);
                p_Decryptor = p_Algorithm.CreateDecryptor(key, iV);
            }
            catch (Exception exc) when (m_Logger.WriteError(exc))
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
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, p_Encryptor, CryptoStreamMode.Write))
                    {
                        byte[] converted = Encoding.UTF8.GetBytes(value);

                        csEncrypt.Write(converted, 0, converted.Length);
                        csEncrypt.FlushFinalBlock();

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, value))
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
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, p_Decryptor, CryptoStreamMode.Write))
                    {
                        byte[] converted = Convert.FromBase64String(value);

                        csDecrypt.Write(converted, 0, converted.Length);
                        csDecrypt.Close();

                        return Encoding.UTF8.GetString(msDecrypt.ToArray());
                    }
                }
            }
            catch (Exception exc) when (m_Logger.WriteError(exc, value))
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
				if (p_Decryptor != null)
				{
					p_Decryptor.Dispose();
					p_Decryptor = null;
				}

				if (p_Encryptor != null)
				{
					p_Encryptor.Dispose();
					p_Encryptor = null;
				}

				if (p_Algorithm != null)
				{
					p_Algorithm.Dispose();
					p_Algorithm = null;
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
        private byte[] ConvertStringToByteArray(string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        /// <summary>
        /// Method to convert a byte array to a unicode string.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>The converted string.</returns>
        private string ConvertByteArrayToString(byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        #endregion
	}
}