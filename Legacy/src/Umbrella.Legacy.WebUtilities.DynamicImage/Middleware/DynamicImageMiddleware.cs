using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.Extensions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.WebUtilities.DynamicImage.Middleware.Options;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Http.Abstractions;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Middleware
{
	/// <summary>
	/// OWIN Middleware that is used to return a dynamically resized version of a source image. The source image and resizing options
	/// are determined by parsing the incoming request URL.
	/// </summary>
	/// <seealso cref="Microsoft.Owin.OwinMiddleware" />
	public class DynamicImageMiddleware : OwinMiddleware
	{
		#region Private Members
		private readonly ILogger _log;
		private readonly IDynamicImageUtility _dynamicImageUtility;
		private readonly IDynamicImageResizer _dynamicImageResizer;
		private readonly IHttpHeaderValueUtility _headerValueUtility;
		private readonly IMimeTypeUtility _mimeTypeUtility;
		private readonly DynamicImageMiddlewareOptions _options;
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicImageMiddleware"/> class.
		/// </summary>
		/// <param name="next">The next middleware.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="dynamicImageUtility">The dynamic image utility.</param>
		/// <param name="dynamicImageResizer">The dynamic image resizer.</param>
		/// <param name="headerValueUtility">The header value utility.</param>
		/// <param name="mimeTypeUtility">The MIME type utility.</param>
		/// <param name="options">The options.</param>
		public DynamicImageMiddleware(
			OwinMiddleware next,
			ILogger<DynamicImageMiddleware> logger,
			IDynamicImageUtility dynamicImageUtility,
			IDynamicImageResizer dynamicImageResizer,
			IHttpHeaderValueUtility headerValueUtility,
			IMimeTypeUtility mimeTypeUtility,
			DynamicImageMiddlewareOptions options)
			: base(next)
		{
			_log = logger;
			_dynamicImageUtility = dynamicImageUtility;
			_dynamicImageResizer = dynamicImageResizer;
			_headerValueUtility = headerValueUtility;
			_mimeTypeUtility = mimeTypeUtility;
			_options = options;
		}
		#endregion

		#region Overridden Methods		
		/// <summary>
		/// Process an individual request.
		/// </summary>
		/// <param name="context">The current <see cref="IOwinContext"/>.</param>
		/// <exception cref="UmbrellaWebException">An error has occurred whilst executing the request.</exception>
		public override async Task Invoke(IOwinContext context)
		{
			context.Request.CallCancelled.ThrowIfCancellationRequested();

			var cts = CancellationTokenSource.CreateLinkedTokenSource(context.Request.CallCancelled);
			CancellationToken token = cts.Token;

			try
			{
				string path = context.Request.Path.Value;

				DynamicImageFormat? overrideFormat = null;

				if (_options.EnableJpgPngWebPOverride
					&& (path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
					&& context.Request.AcceptsWebP())
				{
					overrideFormat = DynamicImageFormat.WebP;
				}

				var (status, imageOptions) = _dynamicImageUtility.TryParseUrl(_options.DynamicImagePathPrefix, path, overrideFormat);

				if (status == DynamicImageParseUrlResult.Skip)
				{
					await Next.Invoke(context);
					return;
				}

				if (status == DynamicImageParseUrlResult.Invalid)
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotFound);
					return;
				}

				DynamicImageMiddlewareMapping mapping = _options.GetMapping(imageOptions.SourcePath);

				if (mapping == null || (mapping.EnableValidation && !_dynamicImageUtility.ImageOptionsValid(imageOptions, mapping.ValidMappings)))
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotFound);
					return;
				}

				DynamicImageItem image = await _dynamicImageResizer.GenerateImageAsync(mapping.FileProviderMapping.FileProvider, imageOptions, token);

				if (image == null)
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotFound);
					return;
				}

				//Check the cache headers
				if (context.Request.IfModifiedSinceHeaderMatched(image.LastModified))
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotModified);
					return;
				}

				string eTagValue = _headerValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);

				if (context.Request.IfNoneMatchHeaderMatched(eTagValue))
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotModified);
					return;
				}

				if (image.Length > 0)
				{
					context.Response.ContentType = _mimeTypeUtility.GetMimeType(image.ImageOptions.Format.ToFileExtensionString());
					context.Response.ContentLength = image.Length;

					if (mapping.Cacheability == MiddlewareHttpCacheability.NoCache)
					{
						context.Response.Headers["Last-Modified"] = _headerValueUtility.CreateLastModifiedHeaderValue(image.LastModified);
						context.Response.ETag = _headerValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);
						context.Response.Headers["Cache-Control"] = "no-cache";
					}
					else
					{
						context.Response.Headers["Cache-Control"] = "no-store";
					}

					await image.WriteContentToStreamAsync(context.Response.Body, token);

					// Ensure the response stream is flushed async immediately here. If not, there could be content
					// still buffered which will not be sent out until the stream is disposed at which point
					// the IO will happen synchronously!
					await context.Response.Body.FlushAsync(token);

					return;
				}
				else
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotFound);
					return;
				}
			}
			catch (Exception exc) when (_log.WriteError(exc, message: "Error in DynamicImageModule for path: " + context.Request.Path, returnValue: true))
			{
				cts.Cancel();
				context.Response.SendStatusCode(HttpStatusCode.NotFound);
				return;
			}
			finally
			{
				cts.Dispose();
			}
		}
		#endregion
	}
}