using Microsoft.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using System.Net;
using System.Web.Configuration;
using Umbrella.Legacy.WebUtilities.DynamicImage.Configuration;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.DynamicImage.Middleware.Options;
using Umbrella.Utilities;
using Umbrella.WebUtilities.Http;
using System.Threading;
using System.Collections.Generic;
using Umbrella.Legacy.WebUtilities.Extensions;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Middleware
{
	public class DynamicImageMiddleware : OwinMiddleware
	{
		#region Private Static Members
		private static readonly List<string> _registeredDynamicImagePathPrefixList = new List<string>();
		#endregion

		#region Private Members
		private readonly ILogger _log;
		private readonly IDynamicImageUtility _dynamicImageUtility;
		private readonly IDynamicImageResizer _dynamicImageResizer;
		private readonly IHttpHeaderValueUtility _headerValueUtility;
		private readonly Lazy<DynamicImageConfigurationOptions> _configurationOptions = new Lazy<DynamicImageConfigurationOptions>(LoadConfigurationOptions);
		private readonly DynamicImageMiddlewareOptions _options;
		#endregion

		#region Private Properties
		private DynamicImageConfigurationOptions ConfigurationOptions => _configurationOptions.Value;
		#endregion

		#region Constructors
		public DynamicImageMiddleware(OwinMiddleware next,
			ILogger<DynamicImageMiddleware> logger,
			IDynamicImageUtility dynamicImageUtility,
			IDynamicImageResizer dynamicImageResizer,
			IHttpHeaderValueUtility headerValueUtility,
			DynamicImageMiddlewareOptions options)
			: base(next)
		{
			_log = logger;
			_dynamicImageUtility = dynamicImageUtility;
			_dynamicImageResizer = dynamicImageResizer;
			_headerValueUtility = headerValueUtility;
			_options = options;

			Guard.ArgumentNotNull(_options.SourceFileProvider, nameof(_options.SourceFileProvider));
			Guard.ArgumentNotNullOrWhiteSpace(_options.DynamicImagePathPrefix, nameof(_options.DynamicImagePathPrefix));

			//Ensure that only one instance of the middleware can be registered for a specified path prefix value
			if (_registeredDynamicImagePathPrefixList.Contains(_options.DynamicImagePathPrefix, StringComparer.OrdinalIgnoreCase))
				throw new DynamicImageException($"The application is trying to register multiple instances of the {nameof(DynamicImageMiddleware)} with the same prefix: {_options.DynamicImagePathPrefix}. This is not allowed.");

			_registeredDynamicImagePathPrefixList.Add(_options.DynamicImagePathPrefix);
		}
		#endregion

		#region Overridden Methods
		public override async Task Invoke(IOwinContext context)
		{
			context.Request.CallCancelled.ThrowIfCancellationRequested();

			var cts = CancellationTokenSource.CreateLinkedTokenSource(context.Request.CallCancelled);
			CancellationToken token = cts.Token;

			try
			{
				string path = context.Request.Path.Value;

				DynamicImageFormat? overrideFormat = null;

				if(_options.EnableJpgPngWebPOverride
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

				if (status == DynamicImageParseUrlResult.Invalid || !_dynamicImageUtility.ImageOptionsValid(imageOptions, ConfigurationOptions))
				{
					cts.Cancel();
					await context.Response.SendStatusCode(HttpStatusCode.NotFound);
					return;
				}
				
				DynamicImageItem image = await _dynamicImageResizer.GenerateImageAsync(_options.SourceFileProvider, imageOptions, token);

				if (image == null)
				{
					cts.Cancel();
					await context.Response.SendStatusCode(HttpStatusCode.NotFound);
					return;
				}

				//Check the cache headers
				if (context.Request.IfModifiedSinceHeaderMatched(image.LastModified))
				{
					cts.Cancel();
					await context.Response.SendStatusCode(HttpStatusCode.NotModified);
					return;
				}

				string eTagValue = _headerValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);

				if (context.Request.IfNoneMatchHeaderMatched(eTagValue))
				{
					cts.Cancel();
					await context.Response.SendStatusCode(HttpStatusCode.NotModified);
					return;
				}

				if (image.Length > 0)
				{
					AppendResponseHeaders(context.Response, image);

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
					await context.Response.SendStatusCode(HttpStatusCode.NotFound);
					return;
				}
			}
			catch (Exception exc) when (_log.WriteError(exc, message: "Error in DynamicImageModule for path: " + context.Request.Path, returnValue: true))
			{
				cts.Cancel();
				await context.Response.SendStatusCode(HttpStatusCode.NotFound);
				return;
			}
		}
		#endregion

		#region Private Methods
		private void AppendResponseHeaders(IOwinResponse response, DynamicImageItem image)
		{
			response.ContentType = "image/" + image.ImageOptions.Format.ToString().ToLowerInvariant();
			response.ContentLength = image.Length;
			response.Headers["Last-Modified"] = _headerValueUtility.CreateLastModifiedHeaderValue(image.LastModified);
			response.ETag = _headerValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);

			if (!string.IsNullOrWhiteSpace(_options.CacheControlHeaderValue))
				response.Headers["Cache-Control"] = _options.CacheControlHeaderValue.Trim().ToLowerInvariant();
		}

		private static DynamicImageConfigurationOptions LoadConfigurationOptions()
		{
			var mappingsConfig = new DynamicImageMappingsConfig(WebConfigurationManager.OpenWebConfiguration("~/web.config"));
			var options = (DynamicImageConfigurationOptions)mappingsConfig;

			return options;
		}
		#endregion
	}
}