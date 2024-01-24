using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Umbrella.AzureServiceBus.Abstractions;
using Umbrella.AzureServiceBus.Exceptions;

namespace Umbrella.AzureServiceBus;

/// <summary>
/// An extended implementation of <see cref="ServiceBusClient"/> that adds support for plugins.
/// </summary>
public class UmbrellaAzureServiceBusClient : ServiceBusClient
{
	private readonly ILoggerFactory _loggerFactory;
	private readonly ILogger _logger;
	private readonly IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin> _plugins = Array.Empty<IUmbrellaAzureServiceBusPlugin>();

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaAzureServiceBusClient"/> class.
	/// </summary>
	/// <param name="loggerFactory">The logger factory.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <param name="options">The options.</param>
	/// <param name="plugins">The plugins.</param>
	public UmbrellaAzureServiceBusClient(
		ILoggerFactory loggerFactory,
		string connectionString,
		ServiceBusClientOptions? options = null,
		IReadOnlyCollection<IUmbrellaAzureServiceBusPlugin>? plugins = null)
		: base(connectionString, options ?? new())
	{
		_loggerFactory = loggerFactory;
		_logger = loggerFactory.CreateLogger<UmbrellaAzureServiceBusClient>();

		if (plugins is not null)
			_plugins = plugins;
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusSender CreateSender(string queueOrTopicName)
	{
		try
		{
			return new UmbrellaAzureServiceBusSender(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusSender>(), this, queueOrTopicName, null, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { queueOrTopicName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create sender.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusSender CreateSender(string queueOrTopicName, ServiceBusSenderOptions options)
	{
		try
		{
			return new UmbrellaAzureServiceBusSender(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusSender>(), this, queueOrTopicName, options, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { queueOrTopicName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create sender.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusReceiver CreateReceiver(string queueName)
	{
		try
		{
			return new UmbrellaAzureServiceBusReceiver(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusReceiver>(), this, queueName, null, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { queueName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create receiver.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusReceiver CreateReceiver(string queueName, ServiceBusReceiverOptions options)
	{
		try
		{
			return new UmbrellaAzureServiceBusReceiver(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusReceiver>(), this, queueName, options, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { queueName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create receiver.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusReceiver CreateReceiver(string topicName, string subscriptionName)
	{
		try
		{
			return new UmbrellaAzureServiceBusReceiver(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusReceiver>(), this, topicName, subscriptionName, null, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { topicName, subscriptionName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create receiver.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusReceiver CreateReceiver(string topicName, string subscriptionName, ServiceBusReceiverOptions options)
	{
		try
		{
			return new UmbrellaAzureServiceBusReceiver(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusReceiver>(), this, topicName, subscriptionName, options, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { topicName, subscriptionName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create receiver.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusProcessor CreateProcessor(string queueName)
	{
		try
		{
			return new UmbrellaAzureServiceBusProcessor(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusProcessor>(), this, queueName, null, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { queueName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create processor.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusProcessor CreateProcessor(string queueName, ServiceBusProcessorOptions options)
	{
		try
		{
			return new UmbrellaAzureServiceBusProcessor(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusProcessor>(), this, queueName, options, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { queueName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create processor.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusProcessor CreateProcessor(string topicName, string subscriptionName)
	{
		try
		{
			return new UmbrellaAzureServiceBusProcessor(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusProcessor>(), this, topicName, subscriptionName, null, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { topicName, subscriptionName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create processor.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusProcessor CreateProcessor(string topicName, string subscriptionName, ServiceBusProcessorOptions options)
	{
		try
		{
			return new UmbrellaAzureServiceBusProcessor(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusProcessor>(), this, topicName, subscriptionName, options, _plugins);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { topicName, subscriptionName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create processor.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusSessionProcessor CreateSessionProcessor(string queueName, ServiceBusSessionProcessorOptions? options = null)
	{
		try
		{
			return new UmbrellaAzureServiceBusSessionProcessor(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusSessionProcessor>(), this, queueName, options);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { queueName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create session processor.", exc);
		}
	}

	/// <inheritdoc />
	public override UmbrellaAzureServiceBusSessionProcessor CreateSessionProcessor(string topicName, string subscriptionName, ServiceBusSessionProcessorOptions? options = null)
	{
		try
		{
			return new UmbrellaAzureServiceBusSessionProcessor(_loggerFactory.CreateLogger<UmbrellaAzureServiceBusSessionProcessor>(), this, topicName, subscriptionName, options);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { topicName, subscriptionName }))
		{
			throw new UmbrellaAzureServiceBusException("Failed to create session processor.", exc);
		}
	}
}