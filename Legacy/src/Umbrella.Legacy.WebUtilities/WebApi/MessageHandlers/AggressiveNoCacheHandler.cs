using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.WebApi.MessageHandlers;

/// <summary>
/// A WebAPI delegating message handler to ensure responses are not cached by clients.
/// </summary>
public class AggressiveNoCacheHandler : DelegatingHandler
{
	/// <inheritdoc />
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

		if (response.IsSuccessStatusCode)
		{
			// Set standard HTTP/1.0 no-cache header (no-store, no-cache, must-revalidate)
			// Set IE extended HTTP/1.1 no-cache headers (post-check, pre-check)
			response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");

			// Set standard HTTP/1.0 no-cache header.
			response.Headers.Add("Pragma", "no-cache");
		}

		return response;
	}
}