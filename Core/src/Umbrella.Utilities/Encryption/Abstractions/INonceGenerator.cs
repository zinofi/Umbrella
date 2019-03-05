using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Encryption.Abstractions
{
	/// <summary>
	/// Generates cryptographically strong nonces.
	/// </summary>
	public interface INonceGenerator
    {
		/// <summary>
		/// Generates a nonce encoded as a base64 string according to the specified length in bytes.
		/// </summary>
		/// <param name="lengthInBytes">The length in bytes.</param>
		/// <returns>A base64 encoded string.</returns>
		string Generate(int lengthInBytes);
    }
}