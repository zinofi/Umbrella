using Azure.Messaging.ServiceBus;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.AzureServiceBus.Abstractions;
using Umbrella.AzureServiceBus.Exceptions;

namespace Umbrella.AzureServiceBus;

/// <summary>
/// An extended implementation of <see cref="ServiceBusSender"/> that adds support for plugins.
/// </summary>
public class UmbrellaAzureServiceBusSender : ServiceBusSender
{
	private readonly IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin> _plugins = Array.Empty<IUmbrellaAzureServiceBusPlugin>();
	private readonly ILogger<UmbrellaAzureServiceBusSender> _logger;

	internal UmbrellaAzureServiceBusSender(
		ILogger<UmbrellaAzureServiceBusSender> logger,
		UmbrellaAzureServiceBusClient client,
		string queueOrTopicName,
		ServiceBusSenderOptions? options = null,
		IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin>? plugins = null)
		: base(client, queueOrTopicName, options ?? new())
	{
		if (plugins is not null)
			_plugins = plugins;

		_logger = logger;
	}

	/// <inheritdoc />
	public override Task CancelScheduledMessageAsync(long sequenceNumber, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	public override Task CancelScheduledMessagesAsync(IEnumerable<long> sequenceNumbers, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	public override ValueTask<ServiceBusMessageBatch> CreateMessageBatchAsync(CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	public override ValueTask<ServiceBusMessageBatch> CreateMessageBatchAsync(CreateMessageBatchOptions options, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	public override Task<long> ScheduleMessageAsync(ServiceBusMessage message, DateTimeOffset scheduledEnqueueTime, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	public override Task<IReadOnlyList<long>> ScheduleMessagesAsync(IEnumerable<ServiceBusMessage> messages, DateTimeOffset scheduledEnqueueTime, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	/// <remarks>NB: We override this method instead of the <![CDATA[SendMessagesAsync(IEnumerable<ServiceBusMessage>, CancellationToken)]]> method because internally that actually calls this method.</remarks>
	public override async Task SendMessagesAsync(IEnumerable<ServiceBusMessage> messages, CancellationToken cancellationToken = default)
	{
		Guard.IsNotNull(messages);
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			foreach (var message in messages)
			{
				foreach (IUmbrellaAzureServiceBusPlugin plugin in _plugins)
				{
					await plugin.OnSendMessageAsync(message, cancellationToken);
				}
			}

			await base.SendMessagesAsync(messages, cancellationToken);
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaAzureServiceBusException("An error occurred while attempting to send a message.", exc);
		}
	}

	/// <inheritdoc />
	public override Task SendMessagesAsync(ServiceBusMessageBatch messageBatch, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}
}