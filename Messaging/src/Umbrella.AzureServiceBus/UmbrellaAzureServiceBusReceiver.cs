using Azure.Messaging.ServiceBus;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.AzureServiceBus.Abstractions;
using Umbrella.AzureServiceBus.Exceptions;

namespace Umbrella.AzureServiceBus;

/// <summary>
/// An extended implementation of <see cref="ServiceBusReceiver"/> that adds support for plugins.
/// </summary>
public class UmbrellaAzureServiceBusReceiver : ServiceBusReceiver
{
	private readonly IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin> _plugins = Array.Empty<IUmbrellaAzureServiceBusPlugin>();
	private readonly ILogger<UmbrellaAzureServiceBusReceiver> _logger;

	internal UmbrellaAzureServiceBusReceiver(
		ILogger<UmbrellaAzureServiceBusReceiver> logger,
		UmbrellaAzureServiceBusClient client,
		string queueName,
		ServiceBusReceiverOptions? options = null,
		IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin>? plugins = null)
		: base(client, queueName, options ?? new())
	{
		if (plugins is not null)
			_plugins = plugins;

		_logger = logger;
	}

	internal UmbrellaAzureServiceBusReceiver(
		ILogger<UmbrellaAzureServiceBusReceiver> logger,
		UmbrellaAzureServiceBusClient client,
		string topicName,
		string subscriptionName,
		ServiceBusReceiverOptions? options = null,
		IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin>? plugins = null)
		: base(client, topicName, subscriptionName, options ?? new())
	{
		if (plugins is not null)
			_plugins = plugins;

		_logger = logger;
	}

	/// <inheritdoc />
	public override async Task CompleteMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(message);

		try
		{
			await base.CompleteMessageAsync(message, cancellationToken);

			foreach (IUmbrellaAzureServiceBusPlugin plugin in _plugins)
			{
				await plugin.OnCompleteMessageAsync(message, cancellationToken);
			}
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaAzureServiceBusException("An error occurred while attempting to complete a message.", exc);
		}
	}

	/// <inheritdoc />
	public override async Task<ServiceBusReceivedMessage> PeekMessageAsync(long? fromSequenceNumber = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			var message = await base.PeekMessageAsync(fromSequenceNumber, cancellationToken);

			foreach (IUmbrellaAzureServiceBusPlugin plugin in _plugins)
			{
				await plugin.OnReceiveMessageAsync(message, cancellationToken);
			}

			return message;
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { fromSequenceNumber }))
		{
			throw new UmbrellaAzureServiceBusException("An error occurred while attempting to peek a message.", exc);
		}
	}

	/// <inheritdoc />
	public override async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessagesAsync(int maxMessages, long? fromSequenceNumber = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			var messages = await base.PeekMessagesAsync(maxMessages, fromSequenceNumber, cancellationToken);

			foreach (var message in messages)
			{
				if (message is null)
					continue;

				foreach (IUmbrellaAzureServiceBusPlugin plugin in _plugins)
				{
					await plugin.OnReceiveMessageAsync(message, cancellationToken);
				}
			}

			return messages;
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { maxMessages, fromSequenceNumber }))
		{
			throw new UmbrellaAzureServiceBusException("An error occurred while attempting to peek messages.", exc);
		}
	}

	/// <inheritdoc />
	public override Task<ServiceBusReceivedMessage> ReceiveDeferredMessageAsync(long sequenceNumber, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	public override Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveDeferredMessagesAsync(IEnumerable<long> sequenceNumbers, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	public override IAsyncEnumerable<ServiceBusReceivedMessage> ReceiveMessagesAsync(CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	/// <remarks>We override this method instead of the ReceiveMessageAsync(TimeSpan?, CancellationToken) method because internally that actually calls this method.</remarks>
	public override async Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveMessagesAsync(int maxMessages, TimeSpan? maxWaitTime = null, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			var messages = await base.ReceiveMessagesAsync(maxMessages, maxWaitTime, cancellationToken);

			foreach (var message in messages)
			{
				if (message is null)
					continue;

				foreach (IUmbrellaAzureServiceBusPlugin plugin in _plugins)
				{
					await plugin.OnReceiveMessageAsync(message, cancellationToken);
				}
			}

			return messages;
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { maxWaitTime }))
		{
			throw new UmbrellaAzureServiceBusException("An error occurred while attempting to receive a message.", exc);
		}
	}
}