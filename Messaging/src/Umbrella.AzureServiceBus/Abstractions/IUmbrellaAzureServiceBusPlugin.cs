using Azure.Messaging.ServiceBus;

namespace Umbrella.AzureServiceBus.Abstractions;

/// <summary>
/// An interface that defines the contract for a plugin that can be used with the Umbrella.AzureServiceBus library.
/// </summary>
public interface IUmbrellaAzureServiceBusPlugin
{
	/// <summary>
	/// Called when a message is sent to the Service Bus.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	Task OnSendMessageAsync(ServiceBusMessage message, CancellationToken cancellationToken = default);

	/// <summary>
	/// Called when a message is received from the Service Bus.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	Task OnReceiveMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default);

	/// <summary>
	/// Called when a message is completed on the Service Bus.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	Task OnCompleteMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default);
}