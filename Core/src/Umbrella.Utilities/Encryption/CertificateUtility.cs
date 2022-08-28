// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Umbrella.Utilities.Encryption.Abstractions;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Encryption;

/// <summary>
/// Utility class to find X509Certificate2 and export them into byte arrays
/// </summary>
public class CertificateUtility : ICertificateUtility
{
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="CertificateUtility"/> class.
	/// </summary>
	/// <param name="loggerFactory">The logger factory.</param>
	public CertificateUtility(ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<CertificateUtility>();
	}

	/// <inheritdoc />
	public X509Certificate2 FindCertificateByThumbprint(StoreName storeName, StoreLocation storeLocation, string thumbprint, bool validationRequired)
	{
		Guard.IsNotNullOrWhiteSpace(thumbprint, nameof(thumbprint));

		var store = new X509Store(storeName, storeLocation);

		try
		{
			store.Open(OpenFlags.ReadOnly);

			var col = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validationRequired);

			return col is null || col.Count is 0 ? throw new ArgumentException("The certificate was not found in the store") : col[0];
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { storeName, storeLocation, validationRequired }))
		{
			throw new UmbrellaException("There has been a problem finding the certificate.", exc);
		}
		finally
		{
			store.Dispose();
		}
	}

	/// <inheritdoc />
	public X509Certificate2 FindCertificateByThumbprint(string thumbprint, bool validateCertificate) => FindCertificateByThumbprint(StoreName.My, StoreLocation.CurrentUser, thumbprint, validateCertificate);

	/// <inheritdoc />
	public byte[] ExportCertificateWithPrivateKey(X509Certificate2 cert, out string password)
	{
		Guard.IsNotNull(cert, nameof(cert));

		try
		{
			password = Convert.ToBase64String(Encoding.Unicode.GetBytes(Guid.NewGuid().ToString("N")));

			return cert.Export(X509ContentType.Pkcs12, password);
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaException("There has been a problem exporting the certificate.", exc);
		}
	}
}