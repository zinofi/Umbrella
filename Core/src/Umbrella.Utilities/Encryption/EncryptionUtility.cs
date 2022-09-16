using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Encryption.Abstractions;

namespace Umbrella.Utilities.Encryption
{
	/// <summary>
	/// An abstract base class used to encapsulate functionality common to derived encryption utilities.
	/// </summary>
	/// <typeparam name="TSymmetricAlgorithm">The type of the algorithm.</typeparam>
	/// <seealso cref="IEncryptionUtility" />
	/// <seealso cref="System.IDisposable" />
	public abstract class EncryptionUtility<TSymmetricAlgorithm> : IEncryptionUtility, IDisposable
		where TSymmetricAlgorithm : SymmetricAlgorithm, new()
	{
		#region Protected Properties		
		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the algorithm.
		/// </summary>
		protected TSymmetricAlgorithm? Algorithm { get; private set; }

		/// <summary>
		/// Gets the encryptor.
		/// </summary>
		protected ICryptoTransform? Encryptor { get; private set; }

		/// <summary>
		/// Gets the decryptor.
		/// </summary>
		protected ICryptoTransform? Decryptor { get; private set; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="EncryptionUtility{TSymmetricAlgorithm}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public EncryptionUtility(ILogger logger)
		{
			Logger = logger;
		}
		#endregion

		#region Internal Methods
		internal byte[] Encrypt(string value)
		{
			try
			{
				using var msEncrypt = new MemoryStream();
				using var csEncrypt = new CryptoStream(msEncrypt, Encryptor, CryptoStreamMode.Write);

				byte[] converted = ConvertStringToByteArray(value);

				csEncrypt.Write(converted, 0, converted.Length);
				csEncrypt.FlushFinalBlock();

				return msEncrypt.ToArray();
			}
			catch (Exception exc) when (Logger.WriteError(exc, value))
			{
				throw;
			}
		}

		internal string Decrypt(byte[] value)
		{
			try
			{
				using var msDecrypt = new MemoryStream();
				using var csDecrypt = new CryptoStream(msDecrypt, Decryptor, CryptoStreamMode.Write);

				csDecrypt.Write(value, 0, value.Length);
				csDecrypt.Close();

				return ConvertByteArrayToString(msDecrypt.ToArray());
			}
			catch (Exception exc) when (Logger.WriteError(exc))
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
				Algorithm = new TSymmetricAlgorithm()
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
			catch (Exception exc) when (Logger.WriteError(exc))
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
				using var msEncrypt = new MemoryStream();
				using var csEncrypt = new CryptoStream(msEncrypt, Encryptor, CryptoStreamMode.Write);

				byte[] converted = Encoding.UTF8.GetBytes(value);

				csEncrypt.Write(converted, 0, converted.Length);
				csEncrypt.FlushFinalBlock();

				return Convert.ToBase64String(msEncrypt.ToArray());
			}
			catch (Exception exc) when (Logger.WriteError(exc, value))
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
				using var msDecrypt = new MemoryStream();
				using var csDecrypt = new CryptoStream(msDecrypt, Decryptor, CryptoStreamMode.Write);

				byte[] converted = Convert.FromBase64String(value);

				csDecrypt.Write(converted, 0, converted.Length);
				csDecrypt.Close();

				return Encoding.UTF8.GetString(msDecrypt.ToArray());
			}
			catch (Exception exc) when (Logger.WriteError(exc, value))
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
				if (Decryptor is not null)
				{
					Decryptor.Dispose();
					Decryptor = null;
				}

				if (Encryptor is not null)
				{
					Encryptor.Dispose();
					Encryptor = null;
				}

				if (Algorithm is not null)
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