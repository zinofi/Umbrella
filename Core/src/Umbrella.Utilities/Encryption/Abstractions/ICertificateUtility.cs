using System.Security.Cryptography.X509Certificates;

namespace Umbrella.Utilities.Encryption.Abstractions;

/// <summary>
/// A utulity used to find certificates
/// </summary>
public interface ICertificateUtility
{
	/// <summary>
	///  Finds the certificate having thumbprint supplied from store location supplied.
	/// </summary>
	/// <param name="storeName">Name of the store.</param>
	/// <param name="storeLocation">The store location.</param>
	/// <param name="thumbprint">The thumbprint.</param>
	/// <param name="validationRequired">if set to <see langword="true"/> specifies that the certificate should be validated.</param>
	/// <returns>The certificate.</returns>
	X509Certificate2 FindCertificateByThumbprint(StoreName storeName, StoreLocation storeLocation, string thumbprint, bool validationRequired);

	/// <summary>
	/// Finds the certificate having thumbprint supplied defaulting to the personal store of currrent user. 
	/// </summary>
	/// <param name="thumbprint">The thumbprint.</param>
	/// <param name="validateCertificate">if set to <c>true</c> [validate certificate].</param>
	/// <returns>The certificate.</returns>
	X509Certificate2 FindCertificateByThumbprint(string thumbprint, bool validateCertificate);

	/// <summary>
	/// Exports the certificate supplied into a byte array and secures it with a randomly generated password. 
	/// </summary>
	/// <param name="cert">The cert.</param>
	/// <param name="password">The generated password.</param>
	/// <returns>The certificate bytes.</returns>
	byte[] ExportCertificateWithPrivateKey(X509Certificate2 cert, out string password);
}