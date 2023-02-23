﻿using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Umbrella.Utilities.Http.Extensions;

/// <summary>
/// Extension methods for the <see cref="HttpResponseMessage"/>.
/// </summary>
public static class HttpResponseMessageExtensions
{
	/// <summary>
	/// Logs the status code, status message and content of the specified <paramref name="responseMessage"/> to the specified <paramref name="logger"/>
	/// with the specified <paramref name="logLevel"/>.
	/// </summary>
	/// <param name="responseMessage">The response message.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="logLevel">The log level.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable task.</returns>
	public static async ValueTask LogResponseMessageDetailsAsync(this HttpResponseMessage responseMessage, ILogger logger, LogLevel logLevel, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		string? requestUri = responseMessage.RequestMessage?.RequestUri?.ToString();

		string? responseContent = await responseMessage.Content.ReadAsStringAsync();

		_ = logger.Write(logLevel, state: new { requestUri, responseMessage.StatusCode, responseMessage.ReasonPhrase, responseContent });
	}
}