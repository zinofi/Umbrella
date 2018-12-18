using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Encryption.Abstractions
{
    public interface ICertificateUtility
    {
        X509Certificate2 FindCertificateByThumbprint(StoreName storeName, StoreLocation storeLocation, string thumbprint, bool validationRequired);
        X509Certificate2 FindCertificateByThumbprint(string thumbprint, bool validateCertificate);
        byte[] ExportCertificateWithPrivateKey(X509Certificate2 cert, out string password);
    }
}