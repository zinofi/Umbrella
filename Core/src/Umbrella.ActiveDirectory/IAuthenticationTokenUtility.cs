using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Umbrella.ActiveDirectory
{
    public interface IAuthenticationTokenUtility
    {
        Task<string> GetTokenAsyncUsingClientCredential(string authority, string resource, string scope, string clientId, string clientSecret);
        Task<string> GetTokenAsyncUsingClientCertificate(string authority, string resource, string scope, string clientId, StoreName storeName, StoreLocation storeLocation, string certificateThumbprint, bool validateCertificate);
    }
}