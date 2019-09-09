using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Legacy.WebUtilities.Extensions;
using Umbrella.Legacy.WebUtilities.FileSystem.Middleware.Options;
using Umbrella.Utilities;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Http;

namespace Umbrella.Legacy.WebUtilities.FileSystem.Middleware
{
	public class UmbrellaFileProviderMiddleware : OwinMiddleware
	{
		protected ILogger Log { get; }
		protected IHttpHeaderValueUtility HttpHeaderValueUtility { get; }
		protected UmbrellaFileProviderMiddlewareOptions Options { get; }

		public UmbrellaFileProviderMiddleware(
			OwinMiddleware next,
			ILogger<UmbrellaFileProviderMiddleware> logger,
			IHttpHeaderValueUtility httpHeaderValueUtility,
			UmbrellaFileProviderMiddlewareOptions options)
			: base(next)
		{
			Guard.ArgumentNotNull(options, nameof(options));

			Log = logger;
			HttpHeaderValueUtility = httpHeaderValueUtility;
			Options = options;
		}

		public override async Task Invoke(IOwinContext context)
		{
			context.Request.CallCancelled.ThrowIfCancellationRequested();

			try
			{
				string path = context.Request.Path.Value;

				IUmbrellaFileProvider fileProvider = Options.GetFileProvider(path);

				if (fileProvider != null)
				{
					var cts = CancellationTokenSource.CreateLinkedTokenSource(context.Request.CallCancelled);
					CancellationToken token = cts.Token;

					IUmbrellaFileInfo fileInfo = await fileProvider.GetAsync(path, token);

					if (fileInfo == null)
					{
						cts.Cancel();
						await context.Response.SendStatusCode(HttpStatusCode.NotFound);
						return;
					}

					// Check the cache headers
					if (context.Request.IfModifiedSinceHeaderMatched(fileInfo.LastModified.Value.UtcDateTime))
					{
						cts.Cancel();
						await context.Response.SendStatusCode(HttpStatusCode.NotModified);
						return;
					}

					string eTagValue = HttpHeaderValueUtility.CreateETagHeaderValue(fileInfo.LastModified.Value, fileInfo.Length);

					if (context.Request.IfNoneMatchHeaderMatched(eTagValue))
					{
						cts.Cancel();
						await context.Response.SendStatusCode(HttpStatusCode.NotModified);
						return;
					}

					context.Response.ContentType = fileInfo.ContentType;
					context.Response.Headers["Last-Modified"] = HttpHeaderValueUtility.CreateLastModifiedHeaderValue(fileInfo.LastModified.Value);
					context.Response.Headers["ETag"] = eTagValue;
					context.Response.Headers["Cache-Control"] = "no-cache";

					await fileInfo.WriteToStreamAsync(context.Response.Body, token);
					await context.Response.Body.FlushAsync();

					return;
				}
				else
				{
					await Next.Invoke(context);
				}
			}
			catch (Exception exc) when (Log.WriteError(exc, new { Path = context.Request.Path.Value }, returnValue: true))
			{
				throw new UmbrellaWebException("An error has occurred whilst executing the request.", exc);
			}
		}
	}
}