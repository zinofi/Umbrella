using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption.Interfaces;

namespace Umbrella.Extensions.Configuration.Azure.KeyVault
{
    /// <summary>
    /// Extension methods on IConfigurationBuilder for loading configuration from Azure KeyVault
    /// </summary>
    public static class AzureKeyVaultConfigurationExtensions
    {
        /// <summary>
        /// Load the config provider which reads shared secret configuration from the named Azure KeyVault using a Certificate loaded from a specified Certificate Store and Location.
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="certificateUtility"></param>
        /// <param name="appClientId"></param>
        /// <param name="vaultName"></param>
        /// <param name="certificateThumbprint"></param>
        /// <param name="validateCertificate"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="maxSecrets"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddAzureKeyVaultSecrets(this IConfigurationBuilder configurationBuilder, ICertificateUtility certificateUtility, string appClientId, string vaultName, string certificateThumbprint, bool validateCertificate, ILoggerFactory loggerFactory, StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.CurrentUser, int maxSecrets = AzureKeyVaultConfigurationProvider.DefaultMaxSecrets)
        {
            configurationBuilder.Add(new AzureKeyVaultConfigurationProvider(certificateUtility, appClientId, vaultName, certificateThumbprint, validateCertificate, loggerFactory, storeName, storeLocation, maxSecrets));
            return configurationBuilder;
        }

        /// <summary>
        /// Load the config provider which reads shared secret configuration from the named Azure KeyVault using a client secret.
        /// This approach is less secure than using the Certificate approach as it requires the secret to be stored in your application configuration.
        /// If using this approach it is best to store the secret and other KeyVault settings using Environment Variables instead of physical configuration files which
        /// live with your application.
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="appClientId"></param>
        /// <param name="appClientSecret"></param>
        /// <param name="vaultName"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="maxSecrets"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddAzureKeyVaultSecrets(this IConfigurationBuilder configurationBuilder, string appClientId, string appClientSecret, string vaultName, ILoggerFactory loggerFactory, int maxSecrets = AzureKeyVaultConfigurationProvider.DefaultMaxSecrets)
        {
            configurationBuilder.Add(new AzureKeyVaultConfigurationProvider(appClientId, appClientSecret, vaultName, loggerFactory, maxSecrets));
            return configurationBuilder;
        }
    }
}
