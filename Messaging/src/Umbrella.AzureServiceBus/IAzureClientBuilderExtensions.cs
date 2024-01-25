using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;

#pragma warning disable IDE0130
namespace Azure.Core.Extensions;

/// <summary>
/// Extension methods used to add Umbrella retry options to the <see cref="IAzureClientBuilder{TClient,TOptions}"/> client builder.
/// </summary>
public static class IAzureClientBuilderExtensions
{
	/// <summary>
	/// Adds the Umbrella retry options to the <see cref="IAzureClientBuilder{TClient,TOptions}"/> client builder.
	/// </summary>
	/// <typeparam name="TClient">The type of the client.</typeparam>
	/// <typeparam name="TOptions">The type of the options.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <param name="retryMode">The retry mode.</param>
	/// <param name="maxRetries">The maximum retries.</param>
	/// <param name="delayInMilliseconds">The delay in milliseconds.</param>
	/// <param name="maxDelayInMilliseconds">The maximum delay in milliseconds.</param>
	/// <param name="tryTimeoutInMilliseconds">The try timeout in milliseconds.</param>
	/// <returns>The <see cref="IAzureClientBuilder{TClient,TOptions}"/> client builder.</returns>
	public static IAzureClientBuilder<TClient, TOptions> AddUmbrellaRetryOptions<TClient, TOptions>(
		this IAzureClientBuilder<TClient, TOptions> builder,
		ServiceBusRetryMode retryMode = ServiceBusRetryMode.Exponential,
		int maxRetries = 3,
		int delayInMilliseconds = 5000,
		int maxDelayInMilliseconds = 30000,
		int tryTimeoutInMilliseconds = 30000)
		where TClient : class
		where TOptions : ServiceBusClientOptions
	{
		return builder.ConfigureOptions(x =>
		{
			x.RetryOptions = new ServiceBusRetryOptions
			{
				Mode = retryMode,
				MaxRetries = maxRetries,
				Delay = TimeSpan.FromMilliseconds(delayInMilliseconds),
				MaxDelay = TimeSpan.FromMilliseconds(maxDelayInMilliseconds),
				TryTimeout = TimeSpan.FromMilliseconds(tryTimeoutInMilliseconds)
			};
		});
	}
}