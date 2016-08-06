using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption.Interfaces;
using Umbrella.Utilities.Extensions;

namespace Umbrella.ActiveDirectory
{
    public class AuthenticationTokenUtility : IAuthenticationTokenUtility
    {
        private readonly ILogger m_Logger;
        private readonly ICertificateUtility m_CertificateUtility;

        public AuthenticationTokenUtility(ILoggerFactory loggerFactory, ICertificateUtility certificateUtility)
        {
            m_Logger = loggerFactory.CreateLogger<AuthenticationTokenUtility>();
            m_CertificateUtility = certificateUtility;
        }

        public async Task<string> GetTokenAsyncUsingClientCredential(string authority, string resource, string scope, string clientId, string clientSecret)
        {
            try
            {
                var authContext = new AuthenticationContext(authority);

                ClientCredential credential = new ClientCredential(clientId, clientSecret);

                AuthenticationResult result = await authContext.AcquireTokenAsync(resource, credential);

                return result?.AccessToken;
            }
            catch (Exception exc) when (m_Logger.WriteCritical(exc, new { clientId }))
            {
                throw;
            }
        }

        public async Task<string> GetTokenAsyncUsingClientCertificate(string authority, string resource, string scope, string clientId, StoreName storeName, StoreLocation storeLocation, string certificateThumbprint, bool validateCertificate)
        {
            try
            {
                var authContext = new AuthenticationContext(authority);

                var cert = m_CertificateUtility.FindCertificateByThumbprint(storeName, storeLocation, certificateThumbprint, validateCertificate);

                ClientAssertionCertificate assertion = new ClientAssertionCertificate(clientId, cert);

                AuthenticationResult result = await authContext.AcquireTokenAsync(resource, assertion);

                return result?.AccessToken;

            }
            catch (Exception exc) when (m_Logger.WriteCritical(exc, new { clientId }))
            {
                throw;
            }
        }
    }
}