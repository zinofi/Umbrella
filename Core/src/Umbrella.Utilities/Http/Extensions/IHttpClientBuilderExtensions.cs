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
		/// <returns>The builder.</returns>
		public static IHttpClientBuilder AddUmbrellaPolicyHandlers(this IHttpClientBuilder builder)
			=> builder.AddPolicyHandler(HttpPolicies.ErrorAndTimeout).AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5));
	}
}