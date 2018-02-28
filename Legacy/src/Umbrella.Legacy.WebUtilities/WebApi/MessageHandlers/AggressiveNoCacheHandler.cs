using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.WebApi.MessageHandlers
{
    public class AggressiveNoCacheHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");

            return response;
        }
    }
}