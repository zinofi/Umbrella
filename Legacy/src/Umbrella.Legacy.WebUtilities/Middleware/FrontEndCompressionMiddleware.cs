using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Brotli;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Umbrella.Legacy.WebUtilities.Extensions;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting.Abstractions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.WebUtilities.Http.Abstractions;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.Legacy.WebUtilities.Middleware
{
	public class FrontEndCompressionMiddleware : OwinMiddleware
	{
		private static readonly char[] _headerValueSplitters = new[] { ',' };
		private static readonly string _cackeKeyPrefix = $"{typeof(FrontEndCompressionMiddleware).FullName}";
		private static readonly ConcurrentDictionary<string, IFileInfo> _fileInfoDictionary = new ConcurrentDictionary<string, IFileInfo>();

		protected ILogger Log { get; }
		protected IHybridCache Cache { get; }
		protected IUmbrellaHostingEnvironment HostingEnvironment { get; }
		protected IHttpHeaderValueUtility HttpHeaderValueUtility { get; }
		protected IMimeTypeUtility MimeTypeUtility { get; }
		protected FrontEndCompressionMiddlewareOptions Options { get; }

		// Exposed as internal for unit testing / benchmarking mocks
		protected internal IFileProvider FileProvider { get; internal set; }

		public FrontEndCompressionMiddleware(
			OwinMiddleware next,
			ILogger<FrontEndCompressionMiddleware> logger,
			IHybridCache cache,
			IUmbrellaHostingEnvironment hostingEnvironment,
			IHttpHeaderValueUtility httpHeaderValueUtility,
			IMimeTypeUtility mimeTypeUtility,
			FrontEndCompressionMiddlewareOptions options)
			: base(next)
		{
			Log = logger;
			Cache = cache;
			HostingEnvironment = hostingEnvironment;
			HttpHeaderValueUtility = httpHeaderValueUtility;
			MimeTypeUtility = mimeTypeUtility;
			Options = options;

			// File Provider
			FileProvider = new PhysicalFileProvider(hostingEnvironment.MapPath("~/"));
		}

		public override async Task Invoke(IOwinContext context)
		{
			context.Request.CallCancelled.ThrowIfCancellationRequested();

			try
			{
				string path = context.Request.Path.Value.Trim();

				if (Options.FrontEndRootFolderAppRelativePaths.Any(x => path.StartsWith(x, StringComparison.OrdinalIgnoreCase))
					&& Options.TargetFileExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
				{
					var cts = CancellationTokenSource.CreateLinkedTokenSource(context.Request.CallCancelled);
					CancellationToken token = cts.Token;

					IFileInfo fileInfo = GetFileInfo(path);

					if (fileInfo == null)
					{
						cts.Cancel();
						await context.Response.SendStatusCodeAsync(HttpStatusCode.NotFound);
						return;
					}

					if (Options.ResponseCacheEnabled)
					{
						bool shouldCache = Options.ResponseCacheDeterminer?.Invoke(fileInfo) ?? true;

						if (shouldCache)
						{
							// Check Request headers
							if (context.Request.IfModifiedSinceHeaderMatched(fileInfo.LastModified))
							{
								cts.Cancel();
								await context.Response.SendStatusCodeAsync(HttpStatusCode.NotModified);
								return;
							}

							string eTagValue = HttpHeaderValueUtility.CreateETagHeaderValue(fileInfo.LastModified, fileInfo.Length);

							if (context.Request.IfNoneMatchHeaderMatched(eTagValue))
							{
								cts.Cancel();
								await context.Response.SendStatusCodeAsync(HttpStatusCode.NotModified);
								return;
							}

							// Set the Response headers
							context.Response.Headers["Last-Modified"] = HttpHeaderValueUtility.CreateLastModifiedHeaderValue(fileInfo.LastModified);
							context.Response.ETag = eTagValue;

							if (Options.MaxAgeSeconds.HasValue)
								context.Response.Expires = DateTimeOffset.UtcNow.AddSeconds(Options.MaxAgeSeconds.Value);

							var sbCacheControl = new StringBuilder(Options.HttpCacheability.ToCacheControlString());

							if (Options.MaxAgeSeconds.HasValue)
								sbCacheControl.Append(", max-age=" + Options.MaxAgeSeconds);

							if (Options.MustRevalidate)
								sbCacheControl.Append(", must-revalidate");

							context.Response.Headers["Cache-Control"] = sbCacheControl.ToString();
						}
						else
						{
							context.Response.Headers["Cache-Control"] = "no-store";
						}
					}
					else
					{
						context.Response.Headers["Cache-Control"] = Options.HttpCacheability.ToCacheControlString();
					}

					byte[] bytes = null;

					if (Options.CompressionEnabled && context.Request.Headers.TryGetValue(Options.AcceptEncodingHeaderKey, out string[] encodingValues))
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
								lstEncodingValue.AddNotNullTrimToLowerInvariant(part);
							}
						}

						// Allow the consumer to alter the accept-encoding values.
						// This is useful for situations where proxies have incorrectly rewritten encoding headers
						// and we need to check something like the User-Agent value to override the values,
						// e.g. Brotli doesn't work with IE
						Options?.AcceptEncodingModifier?.Invoke(context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.AsEnumerable()), lstEncodingValue);

						string flattenedEncodingHeaders = string.Join(", ", lstEncodingValue).ToUpperInvariant();
						string cacheKey = $"{_cackeKeyPrefix}:{path}:{flattenedEncodingHeaders}";

						(string contentEncoding, byte[] bytes) result = await Cache.GetOrCreateAsync<(string contentEncoding, byte[] bytes)>(cacheKey, async () =>
						{
							string contentEncoding = null;

							using Stream fs = fileInfo.CreateReadStream();
							using var ms = new MemoryStream();

							if (lstEncodingValue.Contains("br", StringComparer.OrdinalIgnoreCase) || lstEncodingValue.Contains("brotli", StringComparer.OrdinalIgnoreCase))
							{
								using (var br = new BrotliStream(ms, CompressionMode.Compress))
								{
									await fs.CopyToAsync(br, Options.BufferSizeBytes, token);
								}

								contentEncoding = "br";
							}
							else if (lstEncodingValue.Contains("gzip", StringComparer.OrdinalIgnoreCase))
							{
								using (var gz = new GZipStream(ms, CompressionMode.Compress))
								{
									await fs.CopyToAsync(gz, Options.BufferSizeBytes, token);
								}

								contentEncoding = "gzip";
							}
							else if (lstEncodingValue.Contains("deflate", StringComparer.OrdinalIgnoreCase))
							{
								using (var deflate = new DeflateStream(ms, CompressionMode.Compress))
								{
									await fs.CopyToAsync(deflate, Options.BufferSizeBytes, token);
								}

								contentEncoding = "deflate";
							}
							else
							{
								// If we get here then we are dealing with an unknown content encoding.
								// Just read the file into memory as it is.
								await fs.CopyToAsync(ms, Options.BufferSizeBytes, token);
							}

							return (contentEncoding, ms.ToArray());
						},
						Options,
						context.Request.CallCancelled,
						() => Options.WatchFiles ? new[] { FileProvider.Watch(path) } : null);

						if (Log.IsEnabled(LogLevel.Debug))
						{
							var logData = new
							{
								PathBase = context.Request.PathBase.Value,
								Path = context.Request.Path.Value,
								UserAgent = context.Request.Headers["User-Agent"],
								OriginalOwinEncodingHeaders = encodingValues,
								// This is here to see if the Owin headers are not being set correctly when they're copied
								// from the AspNet headers collection.
								OriginalAspNetEncodingHeaders = HttpContext.Current?.Request?.Headers?.GetValues(Options.AcceptEncodingHeaderKey),
								TranformedOwinEncodingHeaders = lstEncodingValue,
								CompressionAlgorithmUsed = result.contentEncoding,
								CompressedSize = result.bytes.Length
							};

							Log.WriteDebug(logData);
						}

						bytes = result.bytes;

						if (!string.IsNullOrEmpty(result.contentEncoding))
							context.Response.Headers["Content-Encoding"] = result.contentEncoding;

						// Check if the Accept-Encoding key is different from the standard header key, e.g. when moved into a different
						// header by a proxy. We need to make sure the proxy varies the response by this new header in cases where it might be
						// caching some ouput value (even though in theory it shouldn't be as we have set the Cache-Control to private).
						string varyHeader = "Accept-Encoding";

						if (!Options.AcceptEncodingHeaderKey.Equals(varyHeader, StringComparison.OrdinalIgnoreCase))
							varyHeader += ", " + Options.AcceptEncodingHeaderKey;

						context.Response.Headers["Vary"] = varyHeader;
					}

					if (bytes == null)
					{
						// Getting here means that compression is disabled or there isn't an Accept-Encoding header.
						// Therefore, we have to just read the file as it is and return it
						// as it is stored on disk.
						bytes = await Cache.GetOrCreateAsync($"{_cackeKeyPrefix}:{path}", async () =>
						{
							using Stream fs = fileInfo.CreateReadStream();
							using var ms = new MemoryStream();

							await fs.CopyToAsync(ms, Options.BufferSizeBytes, token);

							return ms.ToArray();
						},
						Options,
						context.Request.CallCancelled,
						() => Options.WatchFiles ? new[] { FileProvider.Watch(path) } : null);
					}

					await context.Response.Body.WriteAsync(bytes, 0, bytes.Length, token);

					// Common headers
					context.Response.ContentType = MimeTypeUtility.GetMimeType(fileInfo.Name);
					context.Response.ContentLength = bytes.LongLength;

					// Ensure the response stream is flushed async immediately here. If not, there could be content
					// still buffered which will not be sent out until the stream is disposed at which point
					// the IO will happen synchronously!
					await context.Response.Body.FlushAsync(token);

					return;
				}

				await Next.Invoke(context);
				return;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { Path = context.Request.Path.Value }))
			{
				throw;
			}
		}

		private IFileInfo GetFileInfo(string path)
		{
			IFileInfo LoadFileInfo()
			{
				IFileInfo fileInfo = FileProvider.GetFileInfo(path);

				return fileInfo.Exists ? fileInfo : null;
			}

			return Options.WatchFiles
				? LoadFileInfo()
				: _fileInfoDictionary.GetOrAdd(path.ToUpperInvariant(), key => LoadFileInfo());
		}
	}
}