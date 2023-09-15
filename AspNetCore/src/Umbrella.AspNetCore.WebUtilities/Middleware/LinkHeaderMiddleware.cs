using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.WebUtilities.Middleware.Options;

namespace Ultimedia.Rtms.Web.Server.Middleware;

/// <summary>
/// Middleware that adds Link headers to outgoing HTML responses for a list of URLs specified
/// using the <see cref="LinkHeaderMiddlewareOptions"/>.
/// </summary>
/// <remarks>
/// URLs are appended as headers as a list with rel=preconnect and rel=dns-prefetch values output for each URL.
/// </remarks>
public class LinkHeaderMiddleware
{
    private readonly ILogger _log;
    private readonly IHybridCache _cache;
    private readonly ICacheKeyUtility _cacheKeyUtility;
    private readonly RequestDelegate _next;
	private readonly LinkHeaderMiddlewareOptions _options;

	/// <summary>
	/// Initializes a new instance of the <see cref="LinkHeaderMiddleware"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="hybridCache">The hybrid cache.</param>
	/// <param name="cacheKeyUtility">The cache key utility.</param>
	/// <param name="next">The next.</param>
	/// <param name="options">The options.</param>
	public LinkHeaderMiddleware(
		ILogger<LinkHeaderMiddleware> logger,
		IHybridCache hybridCache,
		ICacheKeyUtility cacheKeyUtility,
		RequestDelegate next,
		LinkHeaderMiddlewareOptions options)
	{
		_log = logger;
		_cache = hybridCache;
		_cacheKeyUtility = cacheKeyUtility;
		_next = next;
		_options = options;
	}

	/// <summary>
	/// Invokes the middleware in the context of the current request. This method is called by the ASP.NET Core infrastructure.
	/// </summary>
	/// <param name="context">The current <see cref="HttpContext"/>.</param>
	public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            context.Response.OnStarting(() =>
            {
				if (_options.Urls.Count is 0)
					return Task.CompletedTask;

                bool isHtmlResponse = context.Response.ContentType?.Contains("text/html", StringComparison.OrdinalIgnoreCase) is true;

                // Don't bother setting the Link header for non-HTML responses.
                if (isHtmlResponse)
                {
                    string cacheKey = _cacheKeyUtility.Create<LinkHeaderMiddleware>("LinkHeaders");

					static string FormatUrl(string url, string rel) => $"<{url}>; crossorigin; rel={rel}";

					string[] cachedValue = _cache.GetOrCreate(cacheKey, () => _options.Urls.Select(x => FormatUrl(x, "preconnect")).Concat(_options.Urls.Select(x => FormatUrl(x, "dns-prefetch")))).ToArray();

                    context.Response.Headers.AppendList("Link", cachedValue);
                }

                return Task.CompletedTask;
            });
        }
        catch (Exception exc) when (_log.WriteError(exc))
        {
            throw;
        }

		await _next.Invoke(context);
	}
}