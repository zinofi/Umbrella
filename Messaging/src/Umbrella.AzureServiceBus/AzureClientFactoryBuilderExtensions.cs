using Azure.Core.Extensions;
using Azure.Messaging.ServiceBus;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.AzureServiceBus;
using Umbrella.AzureServiceBus.Abstractions;
using Umbrella.AzureServiceBus.Plugins;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.Azure;

/// <summary>
/// Extension methods for adding the Umbrella Azure Service Bus client to the <see cref="AzureClientFactoryBuilder"/> builder.
/// </summary>
public static class AzureClientFactoryBuilderExtensions
{
	/// <summary>
	/// Adds the Umbrella Azure Service Bus client to the <see cref="AzureClientFactoryBuilder"/> builder.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <param name="loggerFactory">The logger factory.</param>
	/// <param name="claimCheckStorageConnectionString">The claim check storage connection string.</param>
	/// <param name="claimCheckStorageContainerName">The claim check storage container name.</param>
	/// <param name="serviceBusConnectionString">The service bus connection string.</param>
	/// <returns>The <see cref="IAzureClientBuilder{UmbrellaAzureServiceBusClient, ServiceBusClientOptions}"/> client builder.</returns>
	public static IAzureClientBuilder<UmbrellaAzureServiceBusClient, ServiceBusClientOptions> AddUmbrellaAzureServiceBusClient(
		this AzureClientFactoryBuilder builder,
		ILoggerFactory loggerFactory,
		string claimCheckStorageConnectionString,
		string claimCheckStorageContainerName,
		string serviceBusConnectionString)
	{
		Guard.IsNotNull(builder);

		return builder.AddClient<UmbrellaAzureServiceBusClient, ServiceBusClientOptions>(options =>
		{
			// Define Plugins
			IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin> plugins =
			[
				new UmbrellaAzureServiceBusClaimCheckPlugin(
					loggerFactory.CreateLogger<UmbrellaAzureServiceBusClaimCheckPlugin>(),
					claimCheckStorageConnectionString,
					claimCheckStorageContainerName)
			];

			return new UmbrellaAzureServiceBusClient(
				loggerFactory,
				serviceBusConnectionString,
				options,
				plugins: plugins);
		})
		.AddUmbrellaRetryOptions();
	}
}