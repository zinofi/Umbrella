using System;
using System.Buffers;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Encryption.Abstractions;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Encryption
{
	/// <summary>
	/// Generates cryptographically strong nonces.
	/// </summary>
	/// <seealso cref="INonceGenerator" />
	/// <seealso cref="IDisposable" />
	public class NonceGenerator : INonceGenerator, IDisposable
	{
		private readonly ILogger _log;
		private readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();

		/// <summary>
		/// Initializes a new instance of the <see cref="NonceGenerator"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public NonceGenerator(ILogger<NonceGenerator> logger)
		{
			_log = logger;
		}

		/// <summary>
		/// Generates a nonce encoded as a base64 string according to the specified length in bytes.
		/// </summary>
		/// <param name="lengthInBytes">The length in bytes.</param>
		/// <returns>A base64 encoded string.</returns>
		/// <exception cref="UmbrellaException">An error has occurred whilst generating the nonce of {lengthInBytes}</exception>
		public string Generate(int lengthInBytes)
		{
			Guard.ArgumentInRange(lengthInBytes, nameof(lengthInBytes), 1, 1024);

			byte[] buffer = null;

			try
			{
				buffer = ArrayPool<byte>.Shared.Rent(lengthInBytes);
				_random.GetBytes(buffer);

				return Convert.ToBase64String(buffer);
			}
			catch (Exception exc) when (_log.WriteError(exc, new { lengthInBytes }, returnValue: true))
			{
				throw new UmbrellaException($"An error has occurred whilst generating the nonce of {lengthInBytes}.", exc);
			}
			finally
			{
				if (buffer != null)
					ArrayPool<byte>.Shared.Return(buffer, true);
			}
		}

		#region IDisposable Support
		private bool _isDisposed = false;

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					_random.Dispose();
				}

				_isDisposed = true;
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() => Dispose(true); // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		#endregion
	}
}