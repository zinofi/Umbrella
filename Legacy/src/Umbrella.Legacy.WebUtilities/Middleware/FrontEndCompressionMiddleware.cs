﻿using Brotli;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text;
using System.Web;
using Umbrella.Legacy.WebUtilities.Extensions;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Hosting.Abstractions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.WebUtilities.Http.Abstractions;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.Legacy.WebUtilities.Middleware;

/// <summary>
/// OWIN Middleware that is used to provide caching and compression of front-end assets.
/// </summary>
/// <seealso cref="OwinMiddleware" />
public class FrontEndCompressionMiddleware : OwinMiddleware
{
	#region Private Static Members
	private static readonly char[] _headerValueSplitters = [','];
	private static readonly ConcurrentDictionary<string, IFileInfo?> _fileInfoDictionary = new();
	#endregion

	#region Private Members
	private readonly ILogger _log;
	private readonly ICacheKeyUtility _cacheKeyUtility;
	private readonly IHybridCache _cache;
	private readonly IHttpHeaderValueUtility _httpHeaderValueUtility;
	private readonly IMimeTypeUtility _mimeTypeUtility;
	private readonly FrontEndCompressionMiddlewareOptions _options;
	#endregion

	#region Internal Properties
	// Exposed as internal for unit testing / benchmarking mocks
	internal IFileProvider FileProvider { get; set; }
	#endregion

	#region Constructors
	/// <summary>
	/// Initializes a new instance of the <see cref="FrontEndCompressionMiddleware"/> class.
	/// </summary>
	/// <param name="next">The next.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="cacheKeyUtility">The cache key utility.</param>
	/// <param name="cache">The cache.</param>
	/// <param name="hostingEnvironment">The hosting environment.</param>
	/// <param name="httpHeaderValueUtility">The HTTP header value utility.</param>
	/// <param name="mimeTypeUtility">The MIME type utility.</param>
	/// <param name="options">The options.</param>
	public FrontEndCompressionMiddleware(
		OwinMiddleware next,
		ILogger<FrontEndCompressionMiddleware> logger,
		ICacheKeyUtility cacheKeyUtility,
		IHybridCache cache,
		IUmbrellaHostingEnvironment hostingEnvironment,
		IHttpHeaderValueUtility httpHeaderValueUtility,
		IMimeTypeUtility mimeTypeUtility,
		FrontEndCompressionMiddlewareOptions options)
		: base(next)
	{
		_log = logger;
		_cacheKeyUtility = cacheKeyUtility;
		_cache = cache;
		_httpHeaderValueUtility = httpHeaderValueUtility;
		_mimeTypeUtility = mimeTypeUtility;
		_options = options;

		// File Provider
		FileProvider = new PhysicalFileProvider(hostingEnvironment.MapPath("~/"));
	}
	#endregion

	#region Overridden Methods
	/// <summary>
	/// Process an individual request.
	/// </summary>
	/// <param name="context">The current <see cref="IOwinContext"/>.</param>
	public override async Task Invoke(IOwinContext context)
	{
		context.Request.CallCancelled.ThrowIfCancellationRequested();

		try
		{
			string path = context.Request.Path.Value.Trim();

			FrontEndCompressionMiddlewareMapping? mapping = _options.GetMapping(path);

			if (mapping is null)
			{
				await Next.Invoke(context);
				return;
			}

			if (mapping.AppRelativeFolderPaths.Any(x => path.StartsWith(x, StringComparison.OrdinalIgnoreCase))
				&& mapping.TargetFileExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
			{
				using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.Request.CallCancelled);
				CancellationToken token = cts.Token;

				IFileInfo? fileInfo = GetFileInfo(path, mapping.WatchFiles);

				if (fileInfo is null)
				{
					cts.Cancel();
					context.Response.SendStatusCode(HttpStatusCode.NotFound);
					return;
				}

				if (mapping.ResponseCacheEnabled)
				{
					bool shouldCache = _options.ResponseCacheDeterminer?.Invoke(fileInfo) ?? true;

					if (shouldCache)
					{
						// Check Request headers
						if (context.Request.IfModifiedSinceHeaderMatched(fileInfo.LastModified))
						{
							cts.Cancel();
							context.Response.SendStatusCode(HttpStatusCode.NotModified);
							return;
						}

						string eTagValue = _httpHeaderValueUtility.CreateETagHeaderValue(fileInfo.LastModified, fileInfo.Length);

						if (context.Request.IfNoneMatchHeaderMatched(eTagValue))
						{
							cts.Cancel();
							context.Response.SendStatusCode(HttpStatusCode.NotModified);
							return;
						}

						// Set the Response headers
						if (mapping.Cacheability == MiddlewareHttpCacheability.NoCache)
						{
							context.Response.Headers["Last-Modified"] = _httpHeaderValueUtility.CreateLastModifiedHeaderValue(fileInfo.LastModified);
							context.Response.ETag = eTagValue;

							context.Response.Headers["Cache-Control"] = mapping.Cacheability.ToCacheControlString();
						}
						else if (mapping.Cacheability == MiddlewareHttpCacheability.Private)
						{
							if (mapping.MaxAgeSeconds.HasValue)
								context.Response.Expires = DateTimeOffset.UtcNow.AddSeconds(mapping.MaxAgeSeconds.Value);

							var sbCacheControl = new StringBuilder(mapping.Cacheability.ToCacheControlString());

							if (mapping.MaxAgeSeconds.HasValue)
								_ = sbCacheControl.Append(", max-age=" + mapping.MaxAgeSeconds);

							if (mapping.MustRevalidate)
								_ = sbCacheControl.Append(", must-revalidate");

							context.Response.Headers["Cache-Control"] = sbCacheControl.ToString();
						}
					}
					else
					{
						context.Response.Headers["Cache-Control"] = MiddlewareHttpCacheability.NoStore.ToCacheControlString();
					}
				}
				else
				{
					context.Response.Headers["Cache-Control"] = MiddlewareHttpCacheability.NoStore.ToCacheControlString();
				}

				byte[]? bytes = null;

				if (mapping.CompressionEnabled && context.Request.Headers.TryGetValue(_options.AcceptEncodingHeaderKey, out string[] encodingValues))
				{
					// Parse the headers
					var lstEncodingValue = new HashSet<string>();

					foreach (string value in encodingValues)
					{
						if (string.IsNullOrWhiteSpace(value))
							continue;

						string[] parts = value.Split(_headerValueSplitters, StringSplitOptions.RemoveEmptyEntries);

						foreach (string part in parts)
						{
							_ = lstEncodingValue.AddNotNullTrimToLowerInvariant(part);
						}
					}

					// Allow the consumer to alter the accept-encoding values.
					// This is useful for situations where proxies have incorrectly rewritten encoding headers
					// and we need to check something like the User-Agent value to override the values,
					// e.g. Brotli doesn't work with IE
					_options.AcceptEncodingModifier?.Invoke(context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.AsEnumerable()), lstEncodingValue);

					string flattenedEncodingHeaders = string.Join(", ", lstEncodingValue).ToUpperInvariant();
					string[]? cacheKeyParts = null;

					try
					{
						cacheKeyParts = ArrayPool<string>.Shared.Rent(2);
						cacheKeyParts[0] = path;
						cacheKeyParts[1] = flattenedEncodingHeaders;

						string cacheKey = _cacheKeyUtility.Create<FrontEndCompressionMiddleware>(cacheKeyParts, 2);

						(string contentEncoding, byte[] bytes) result = await _cache.GetOrCreateAsync(cacheKey, async () =>
						{
							string? contentEncoding = null;

							using Stream fs = fileInfo.CreateReadStream();
							using var ms = new MemoryStream();

							if (lstEncodingValue.Contains("br", StringComparer.OrdinalIgnoreCase) || lstEncodingValue.Contains("brotli", StringComparer.OrdinalIgnoreCase))
							{
								using (var br = new BrotliStream(ms, CompressionMode.Compress))
								{
									await fs.CopyToAsync(br, _options.BufferSizeBytes, token);
								}

								contentEncoding = "br";
							}
							else if (lstEncodingValue.Contains("gzip", StringComparer.OrdinalIgnoreCase))
							{
								using (var gz = new GZipStream(ms, CompressionMode.Compress))
								{
									await fs.CopyToAsync(gz, _options.BufferSizeBytes, token);
								}

								contentEncoding = "gzip";
							}
							else if (lstEncodingValue.Contains("deflate", StringComparer.OrdinalIgnoreCase))
							{
								using (var deflate = new DeflateStream(ms, CompressionMode.Compress))
								{
									await fs.CopyToAsync(deflate, _options.BufferSizeBytes, token);
								}

								contentEncoding = "deflate";
							}
							else
							{
								// If we get here then we are dealing with an unknown content encoding.
								// Just read the file into memory as it is.
								await fs.CopyToAsync(ms, _options.BufferSizeBytes, token);
							}

							return (contentEncoding, ms.ToArray());
						},
						mapping,
						() => mapping.WatchFiles ? new[] { FileProvider.Watch(path) } : null,
						context.Request.CallCancelled);

						if (_log.IsEnabled(LogLevel.Debug))
						{
							var logData = new
							{
								PathBase = context.Request.PathBase.Value,
								Path = context.Request.Path.Value,
								UserAgent = context.Request.Headers["User-Agent"],
								OriginalOwinEncodingHeaders = encodingValues,
								// This is here to see if the Owin headers are not being set correctly when they're copied
								// from the AspNet headers collection.
								OriginalAspNetEncodingHeaders = HttpContext.Current?.Request?.Headers?.GetValues(_options.AcceptEncodingHeaderKey),
								TranformedOwinEncodingHeaders = lstEncodingValue,
								CompressionAlgorithmUsed = result.contentEncoding,
								CompressedSize = result.bytes.Length
							};

							_log.WriteDebug(logData);
						}

						bytes = result.bytes;

						if (!string.IsNullOrEmpty(result.contentEncoding))
							context.Response.Headers["Content-Encoding"] = result.contentEncoding;
					}
					finally
					{
						if (cacheKeyParts is not null)
							ArrayPool<string>.Shared.Return(cacheKeyParts);
					}

					// Check if the Accept-Encoding key is different from the standard header key, e.g. when moved into a different
					// header by a proxy. We need to make sure the proxy varies the response by this new header in cases where it might be
					// caching some ouput value (even though in theory it shouldn't be as we have set the Cache-Control to private).
					string varyHeader = "Accept-Encoding";

					if (!_options.AcceptEncodingHeaderKey.Equals(varyHeader, StringComparison.OrdinalIgnoreCase))
						varyHeader += ", " + _options.AcceptEncodingHeaderKey;

					context.Response.Headers["Vary"] = varyHeader;
				}

				if (bytes is null)
				{
					// Getting here means that compression is disabled or there isn't an Accept-Encoding header.
					// Therefore, we have to just read the file as it is and return it
					// as it is stored on disk.
					string cacheKey = _cacheKeyUtility.Create<FrontEndCompressionMiddleware>(path);

					bytes = await _cache.GetOrCreateAsync(cacheKey, async () =>
					{
						using Stream fs = fileInfo.CreateReadStream();
						using var ms = new MemoryStream();

						await fs.CopyToAsync(ms, _options.BufferSizeBytes, token);

						return ms.ToArray();
					},
					mapping,
					() => mapping.WatchFiles ? new[] { FileProvider.Watch(path) } : null,
					context.Request.CallCancelled);
				}

				// Common headers
				context.Response.ContentType = _mimeTypeUtility.GetMimeType(fileInfo.Name);
				context.Response.ContentLength = bytes.LongLength;

				await context.Response.Body.WriteAsync(bytes, 0, bytes.Length, token);

				// Ensure the response stream is flushed async immediately here. If not, there could be content
				// still buffered which will not be sent out until the stream is disposed at which point
				// the IO will happen synchronously!
				await context.Response.Body.FlushAsync(token);

				return;
			}

			await Next.Invoke(context);
		}
		catch (Exception exc) when (_log.WriteError(exc, new { Path = context.Request.Path.Value }))
		{
			throw;
		}
	}
	#endregion

	#region Private Methods
	private IFileInfo? GetFileInfo(string path, bool watchFiles)
	{
		IFileInfo? LoadFileInfo()
		{
			IFileInfo fileInfo = FileProvider.GetFileInfo(path);

			return fileInfo.Exists ? fileInfo : null;
		}

		return watchFiles
			? LoadFileInfo()
			: _fileInfoDictionary.GetOrAdd(path.ToUpperInvariant(), key => LoadFileInfo());
	}
	#endregion
}