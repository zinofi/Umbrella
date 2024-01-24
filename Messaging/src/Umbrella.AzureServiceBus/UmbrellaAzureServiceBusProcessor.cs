using Azure.Messaging.ServiceBus;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.AzureServiceBus.Abstractions;
using Umbrella.AzureServiceBus.Exceptions;

namespace Umbrella.AzureServiceBus;

/// <summary>
/// An extended implementation of <see cref="ServiceBusProcessor"/> that adds support for plugins.
/// </summary>
public class UmbrellaAzureServiceBusProcessor : ServiceBusProcessor
{
	private readonly IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin> _plugins = Array.Empty<IUmbrellaAzureServiceBusPlugin>();
	private readonly ILogger<UmbrellaAzureServiceBusProcessor> _logger;

	internal UmbrellaAzureServiceBusProcessor(
		ILogger<UmbrellaAzureServiceBusProcessor> logger,
		UmbrellaAzureServiceBusClient client,
		string queueName,
		ServiceBusProcessorOptions? options = null,
		IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin>? plugins = null)
		: base(client, queueName, options ?? new())
	{
		if (plugins is not null)
			_plugins = plugins;

		_logger = logger;
	}

	internal UmbrellaAzureServiceBusProcessor(
		ILogger<UmbrellaAzureServiceBusProcessor> logger,
		UmbrellaAzureServiceBusClient client,
		string topicName,
		string subscriptionName,
		ServiceBusProcessorOptions? options = null,
		IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin>? plugins = null)
		: base(client, topicName, subscriptionName, options ?? new())
	{
		if (plugins is not null)
			_plugins = plugins;

		_logger = logger;
	}

	/// <inheritdoc />
	protected override async Task OnProcessMessageAsync(ProcessMessageEventArgs args)
	{
		Guard.IsNotNull(args);
		args.CancellationToken.ThrowIfCancellationRequested();

		try
		{
			foreach (IUmbrellaAzureServiceBusPlugin plugin in _plugins)
			{
				await plugin.OnReceiveMessageAsync(args.Message, args.CancellationToken);
			}

			await base.OnProcessMessageAsync(args);
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaAzureServiceBusException("An error occurred while attempting to process a message.", exc);
		}
	}
}