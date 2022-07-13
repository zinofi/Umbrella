using System;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;

namespace Umbrella.Utilities.Http
{
	/// <summary>
	/// A collection of default Http Policies using Polly.
	/// </summary>
	public static class HttpPolicies
	{
		private static readonly Random _random = new Random();

		// TODO: need to move to using Random.Shared in .NET 6 as Random.Next() is not thread-safe
		// and will return zero when accessed by multiple threads concurrently.

		/// <summary>
		/// The error and timeout policy.
		/// </summary>
		public static AsyncRetryPolicy<HttpResponseMessage> ErrorAndTimeout = HttpPolicyExtensions.HandleTransientHttpError()
				.Or<TimeoutRejectedException>()
				.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(_random.Next(0, 100)));
	}
}