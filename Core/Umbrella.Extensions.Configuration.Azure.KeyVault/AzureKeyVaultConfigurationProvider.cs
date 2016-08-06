using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using System.Threading;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Umbrella.Utilities.Encryption.Interfaces;
using Umbrella.ActiveDirectory;

namespace Umbrella.Extensions.Configuration.Azure.KeyVault
{
    /// <summary>
    /// Asp.Net configuration provider to read secrets from key vault. This requires List and Get permissions on the vault.
    /// </summary>
    public class AzureKeyVaultConfigurationProvider : ConfigurationProvider, IConfigurationSource
    {
        public const int DefaultMaxSecrets = 25;
        private const string c_ConfigKey = "ConfigKey";
        private const string c_VaultUrlFormat = "https://{0}.vault.azure.net:443/";

        private readonly IAuthenticationTokenUtility m_TokenUtility;
        private readonly string m_AppClientId;
        private readonly string m_AppClientSecret;
        private readonly string m_Vault;
        private readonly string m_CertificateThumbprint;
        private readonly bool m_ValidateCertificate;
        private readonly StoreName m_StoreName;
        private readonly StoreLocation m_StoreLocation;
        private readonly ILogger Log;
        private readonly int m_MaxSecrets;

        private readonly ClientAuthenticationMode m_AuthMode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appClientId"></param>
        /// <param name="appClientSecret"></param>
        /// <param name="vaultName"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="maxSecrets"></param>
        public AzureKeyVaultConfigurationProvider(IAuthenticationTokenUtility tokenUtility, string appClientId, string appClientSecret, string vaultName, ILoggerFactory loggerFactory, int maxSecrets = DefaultMaxSecrets)
        {
            Guard.ArgumentNotNullOrWhiteSpace(appClientId, nameof(appClientId));
            Guard.ArgumentNotNullOrWhiteSpace(appClientSecret, nameof(appClientSecret));
            Guard.ArgumentNotNullOrWhiteSpace(vaultName, nameof(vaultName));
            Guard.ArgumentNotNull(loggerFactory, nameof(loggerFactory));

            m_AppClientId = appClientId;
            m_AppClientSecret = appClientSecret;
            m_Vault = string.Format(c_VaultUrlFormat, vaultName);
            Log = loggerFactory.CreateLogger<AzureKeyVaultConfigurationProvider>();
            m_MaxSecrets = maxSecrets;

            m_AuthMode = ClientAuthenticationMode.ClientCredential;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificateUtility"></param>
        /// <param name="appClientId"></param>
        /// <param name="vaultName"></param>
        /// <param name="certificateThumbprint"></param>
        /// <param name="validateCertificate"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="maxSecrets"></param>
        public AzureKeyVaultConfigurationProvider(IAuthenticationTokenUtility tokenUtility, string appClientId, string vaultName, string certificateThumbprint, bool validateCertificate, ILoggerFactory loggerFactory, StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.CurrentUser, int maxSecrets = DefaultMaxSecrets)
        {
            Guard.ArgumentNotNullOrWhiteSpace(appClientId, nameof(appClientId));
            Guard.ArgumentNotNullOrWhiteSpace(vaultName, nameof(vaultName));
            Guard.ArgumentNotNullOrWhiteSpace(certificateThumbprint, nameof(certificateThumbprint));
            Guard.ArgumentNotNull(loggerFactory, nameof(loggerFactory));

            m_TokenUtility = tokenUtility;
            m_AppClientId = appClientId;
            m_Vault = string.Format(c_VaultUrlFormat, vaultName);
            m_StoreName = storeName;
            m_StoreLocation = storeLocation;
            m_CertificateThumbprint = certificateThumbprint;
            m_ValidateCertificate = validateCertificate;
            Log = loggerFactory.CreateLogger<AzureKeyVaultConfigurationProvider>();
            m_MaxSecrets = maxSecrets;

            m_AuthMode = ClientAuthenticationMode.ClientAssertionCertificate;
        }

        /// <summary>
        /// Loads all secrets which are delimited by : so that they can be retrieved by the config system
        /// Since KeyVault does not  allow the : character as delimiter in the share secret name is not used as key for configuration, the Tag properties are used instead
        /// The tag should always be of the form "ConfigKey"="ParentKey1:Child1:.."
        /// </summary>
        public override void Load()
        {
            try
            {
                LoadAsync(CancellationToken.None).Wait();
                Log.WriteInformation("Configuration loaded successfully for Client: {0}", m_AppClientId);
            }
            catch (Exception exc) when (Log.WriteCritical(exc, new { m_AppClientId }))
            {
                throw;
            }
        }
        /// <summary>
        /// Loads all secrets which are delimited by : so that they can be retrieved by the config system
        /// Since KeyVault does not  allow : as delimiters in the share secret name, the actual name is not used as key for configuration.
        ///  The Tag property is used instead
        /// The tag should always be of the form "ConfigKey"="ParentKey1:Child1:.."
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task LoadAsync(CancellationToken token)
        {
            Data = new Dictionary<string, string>();

            // This returns a list of identifiers which are uris to the secret, you need to use the identifier to get the actual secrets again.
            KeyVaultClient.AuthenticationCallback tokenCallback = (authority, resource, scope) => m_AuthMode == ClientAuthenticationMode.ClientCredential
                ? m_TokenUtility.GetTokenAsyncUsingClientCredential(authority, resource, scope, m_AppClientId, m_AppClientSecret)
                : m_TokenUtility.GetTokenAsyncUsingClientCertificate(authority, resource, scope, m_AppClientId, m_StoreName, m_StoreLocation, m_CertificateThumbprint, m_ValidateCertificate);

            var kvClient = new KeyVaultClient(tokenCallback);

            var secretsResponseList = await kvClient.GetSecretsAsync(m_Vault, m_MaxSecrets, token);
            foreach (var secretItem in secretsResponseList.Value ?? new List<SecretItem>())
            {
                //The actual config key is stored in a tag with the Key "ConfigKey" since : is not supported in a shared secret name by KeyVault
                if (secretItem.Tags != null && secretItem.Tags.ContainsKey(c_ConfigKey))
                {
                    var secret = await kvClient.GetSecretAsync(secretItem.Id, token);
                    Data.Add(secret.Tags[c_ConfigKey], secret.Value);
                }
            }
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return this;
        }
    }
}