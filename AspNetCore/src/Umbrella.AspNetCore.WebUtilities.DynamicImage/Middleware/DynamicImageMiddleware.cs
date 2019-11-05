using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbrella.AspNetCore.WebUtilities.Extensions;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.WebUtilities.DynamicImage.Middleware.Options;
using Umbrella.WebUtilities.Http.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Middleware
{
	public class DynamicImageMiddleware
	{
		#region Private Static Members
		private static readonly List<string> s_RegisteredDynamicImagePathPrefixList = new List<string>();
		#endregion

		#region Private Members
		private readonly RequestDelegate m_Next;
		private readonly ILogger m_Logger;
		private readonly DynamicImageConfigurationOptions m_DynamicImageConfigurationOptions;
		private readonly IDynamicImageUtility m_DynamicImageUtility;
		private readonly IDynamicImageResizer m_DynamicImageResizer;
		private readonly IHostingEnvironment m_HostingEnvironment;
		private readonly IHttpHeaderValueUtility m_HeaderValueUtility;
		private readonly DynamicImageMiddlewareOptions m_MiddlewareOptions = new DynamicImageMiddlewareOptions();
		#endregion

		#region Constructors
		public DynamicImageMiddleware(RequestDelegate next,
			ILogger<DynamicImageMiddleware> logger,
			IOptions<DynamicImageConfigurationOptions> configOptions,
			IDynamicImageUtility dynamicImageUtility,
			IDynamicImageResizer dynamicImageResizer,
			IHostingEnvironment hostingEnvironment,
			IHttpHeaderValueUtility headerValueUtility,
			DynamicImageMiddlewareOptions options)
		{
			m_Next = next;
			m_Logger = logger;
			m_DynamicImageConfigurationOptions = configOptions.Value;
			m_DynamicImageUtility = dynamicImageUtility;
			m_DynamicImageResizer = dynamicImageResizer;
			m_HostingEnvironment = hostingEnvironment;
			m_HeaderValueUtility = headerValueUtility;
			m_MiddlewareOptions = options;

			//Ensure that only one instance of the middleware can be registered for a specified path prefix value
			if (s_RegisteredDynamicImagePathPrefixList.Contains(m_MiddlewareOptions.DynamicImagePathPrefix, StringComparer.OrdinalIgnoreCase))
				throw new DynamicImageException($"The application is trying to register multiple instances of the {nameof(DynamicImageMiddleware)} with the same prefix: {m_MiddlewareOptions.DynamicImagePathPrefix}. This is not allowed.");

			s_RegisteredDynamicImagePathPrefixList.Add(m_MiddlewareOptions.DynamicImagePathPrefix);
		}
		#endregion

		#region Public Methods
		public async Task Invoke(HttpContext context)
		{
			var cts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
			CancellationToken token = cts.Token;

			try
			{
				(DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) = m_DynamicImageUtility.TryParseUrl(m_MiddlewareOptions.DynamicImagePathPrefix, context.Request.Path.Value);

				if (status == DynamicImageParseUrlResult.Skip)
				{
					await m_Next.Invoke(context);
					return;
				}

				if (status == DynamicImageParseUrlResult.Invalid || !m_DynamicImageUtility.ImageOptionsValid(imageOptions, m_DynamicImageConfigurationOptions))
				{
					cts.Cancel();
					SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
					return;
				}

				DynamicImageItem image = await m_DynamicImageResizer.GenerateImageAsync(m_MiddlewareOptions.SourceFileProvider, imageOptions, token);

				if (image == null)
				{
					cts.Cancel();
					SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
					return;
				}

				//Check the cache headers
				if (context.Request.IfModifiedSinceHeaderMatched(image.LastModified))
				{
					cts.Cancel();
					SetResponseStatusCode(context.Response, HttpStatusCode.NotModified);
					return;
				}

				string eTagValue = m_HeaderValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);

				if (context.Request.IfNoneMatchHeaderMatched(eTagValue))
				{
					cts.Cancel();
					SetResponseStatusCode(context.Response, HttpStatusCode.NotModified);
					return;
				}

				if (image.Length > 0)
				{
					AppendResponseHeaders(context.Response, image);

					await image.WriteContentToStreamAsync(context.Response.Body, token);
					return;
				}
				else
				{
					cts.Cancel();
					SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
					return;
				}
			}
			catch (Exception exc) when (m_Logger.WriteError(exc, message: "Error in DynamicImageModule for path: " + context.Request.Path, returnValue: false))
			{
				cts.Cancel();
				SetResponseStatusCode(context.Response, HttpStatusCode.NotFound);
				return;
			}
		}
		#endregion

		#region Private Methods
		private void AppendResponseHeaders(HttpResponse response, DynamicImageItem image)
		{
			response.Headers["Content-Type"] = "image/" + image.ImageOptions.Format.ToString().ToLower();
			response.Headers["Last-Modified"] = m_HeaderValueUtility.CreateLastModifiedHeaderValue(image.LastModified);
			response.Headers["ETag"] = m_HeaderValueUtility.CreateETagHeaderValue(image.LastModified, image.Length);

			if (!string.IsNullOrWhiteSpace(m_MiddlewareOptions.CacheControlHeaderValue))
				response.Headers["Cache-Control"] = m_MiddlewareOptions.CacheControlHeaderValue.Trim().ToLowerInvariant();
		}

		private void SetResponseStatusCode(HttpResponse response, HttpStatusCode statusCode)
		{
			response.Clear();
			response.StatusCode = (int)statusCode;
		}
		#endregion
	}
}