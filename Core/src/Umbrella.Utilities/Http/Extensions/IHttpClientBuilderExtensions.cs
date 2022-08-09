using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Umbrella.Utilities.Http.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="IHttpClientBuilder"/>.
	/// </summary>
	public static class IHttpClientBuilderExtensions
	{
		/// <summary>
		/// Adds the default policy handlers.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="timeout">The request timeout in seconds.</param>
		/// <param name="firstRetryDelaySeconds">The first retry delay in seconds.</param>
		/// <param name="retryCount">The retry count in case of failure.</param>
		/// <returns>The builder.</returns>
		public static IHttpClientBuilder AddUmbrellaPolicyHandlers(this IHttpClientBuilder builder, int timeout = 5, int firstRetryDelaySeconds = 2, int retryCount = 3)
			=> builder.AddPolicyHandler(HttpPolicies.CreateErrorAndTimeoutPolicy(firstRetryDelaySeconds, retryCount)).AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeout));
	}
}