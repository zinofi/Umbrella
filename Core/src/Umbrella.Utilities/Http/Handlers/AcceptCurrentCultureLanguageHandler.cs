using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using CommunityToolkit.Diagnostics;

namespace Umbrella.Utilities.Http.Handlers;

/// <summary>
/// A delegating handler that sets the Accept-Language header to the current culture of the thread.
/// </summary>
public class AcceptCurrentCultureLanguageHandler : DelegatingHandler
{
	/// <inheritdoc />
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		Guard.IsNotNull(request);

		request.Headers.AcceptLanguage.Clear();
		request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(CultureInfo.CurrentCulture.Name));

		return base.SendAsync(request, cancellationToken);
	}
}