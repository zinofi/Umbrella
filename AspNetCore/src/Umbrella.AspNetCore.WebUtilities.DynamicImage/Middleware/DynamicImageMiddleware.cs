using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Extensions;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.WebUtilities.DynamicImage.Middleware.Options;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Http.Abstractions;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Middleware
{
	/// <summary>
	/// Middleware that is used to return a dynamically resized version of a source image. The source image and resizing options
	/// are determined by parsing the incoming request URL.
	/// </summary>
	public class DynamicImageMiddleware
	{
		#region Private Members
		private readonly RequestDelegate _next;
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
			RequestDelegate next,
			ILogger<DynamicImageMiddleware> logger,
			IDynamicImageUtility dynamicImageUtility,
			IDynamicImageResizer dynamicImageResizer,
			IHttpHeaderValueUtility headerValueUtility,
			IMimeTypeUtility mimeTypeUtility,
			DynamicImageMiddlewareOptions options)
		{
			_next = next;
			_log = logger;
			_dynamicImageUtility = dynamicImageUtility;
			_dynamicImageResizer = dynamicImageResizer;
			_headerValueUtility = headerValueUtility;
			_mimeTypeUtility = mimeTypeUtility;
			_options = options;
		}
		#endregion

		#region Public Methods						
		/// <summary>
		/// Process an individual request.
		/// </summary>
		/// <param name="context">The current <see cref="HttpContext"/>.</param>
		public async Task InvokeAsync(HttpContext context)
		{
			context.RequestAborted.ThrowIfCancellationRequested();

			var cts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
			CancellationToken token = cts.Token;

			try
			{
				string? path = context.Request.Path.Value?.Trim();

				if (string.IsNullOrEmpty(path) || !path.StartsWith($"/{_options.DynamicImagePathPrefix}/", StringComparison.OrdinalIgnoreCase))
				{
					await _next.Invoke(context);
					return;
				}

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
					await _next.Invoke(context);
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

				DynamicImageItem? image = await _dynamicImageResizer.GenerateImageAsync(mapping.FileProviderMapping.FileProvider, imageOptions, token);

				if (image == null)
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotFound);
					return;
				}

				//Check the cache headers
				if (image.LastModified.HasValue && context.Request.IfModifiedSinceHeaderMatched(image.LastModified.Value))
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotModified);
					return;
				}

				string? eTagValue = image.LastModified.HasValue
					? _headerValueUtility.CreateETagHeaderValue(image.LastModified.Value, image.Length)
					: null;

				if (eTagValue != null && context.Request.IfNoneMatchHeaderMatched(eTagValue))
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotModified);
					return;
				}

				if (image.Length > 0)
				{
					context.Response.ContentType = _mimeTypeUtility.GetMimeType(image.ImageOptions.Format.ToFileExtensionString());
					context.Response.ContentLength = image.Length;

					if (mapping.Cacheability == MiddlewareHttpCacheability.NoCache && image.LastModified.HasValue)
					{
						context.Response.Headers["Last-Modified"] = _headerValueUtility.CreateLastModifiedHeaderValue(image.LastModified.Value);
						context.Response.Headers["ETag"] = eTagValue;
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
			catch (UmbrellaFileSystemException exc) when (_log.WriteWarning(exc, new { Path = context.Request.Path.Value }, returnValue: true))
			{
				// Just return a 404 NotFound so that any potential attacker isn't even aware the file exists.
				cts.Cancel();
				context.Response.SendStatusCode(HttpStatusCode.NotFound);
			}
			catch (UmbrellaFileAccessDeniedException exc) when (_log.WriteWarning(exc, new { Path = context.Request.Path.Value }, returnValue: true))
			{
				// Just return a 404 NotFound so that any potential attacker isn't even aware the file exists.
				cts.Cancel();
				context.Response.SendStatusCode(HttpStatusCode.NotFound);
			}
			catch (DynamicImageException exc) when (_log.WriteWarning(exc, new { Path = context.Request.Path.Value }, returnValue: true))
			{
				// Just return a 404 NotFound.
				cts.Cancel();
				context.Response.SendStatusCode(HttpStatusCode.NotFound);
			}
			catch (Exception exc) when (_log.WriteError(exc, new { Path = context.Request.Path.Value }, returnValue: true))
			{
				throw new UmbrellaWebException("An error has occurred whilst executing the request.", exc);
			}
			finally
			{
				cts.Dispose();
			}
		}
		#endregion
	}
}