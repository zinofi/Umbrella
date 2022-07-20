using System;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;
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
		/// <summary>
		/// The error and timeout policy.
		/// </summary>
		public static AsyncRetryPolicy<HttpResponseMessage> ErrorAndTimeout = HttpPolicyExtensions.HandleTransientHttpError()
				.Or<TimeoutRejectedException>()
				.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 3));
	}
}