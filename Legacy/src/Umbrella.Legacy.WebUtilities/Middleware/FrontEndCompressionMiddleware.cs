using Brotli;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.Middleware.Options;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Hosting;
using Umbrella.Utilities.Mime;
using Umbrella.Utilities.Primitives;
using Umbrella.WebUtilities.Http;

namespace Umbrella.Legacy.WebUtilities.Middleware
{
    public class FrontEndCompressionMiddleware : OwinMiddleware
    {
        private static readonly string _cackeKeyPrefix = $"{typeof(FrontEndCompressionMiddleware).FullName}";

        protected ILogger Log { get; }
        protected IMultiCache Cache { get; }
        protected IUmbrellaHostingEnvironment HostingEnvironment { get; }
        protected IHttpHeaderValueUtility HttpHeaderValueUtility { get; }
        protected IMimeTypeUtility MimeTypeUtility { get; }
        protected FrontEndCompressionMiddlewareOptions Options { get; }

        public FrontEndCompressionMiddleware(
            OwinMiddleware next,
            ILogger<FrontEndCompressionMiddleware> logger,
            IMultiCache cache,
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

            // Validate the options
            Guard.ArgumentNotNullOrEmpty(options.FrontEndRootFolderAppRelativePaths, nameof(options.FrontEndRootFolderAppRelativePaths));
            Guard.ArgumentNotNullOrEmpty(options.TargetFileExtensions, nameof(options.TargetFileExtensions));

            // Clean paths
            HashSet<string> lstCleanedPath = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < options.FrontEndRootFolderAppRelativePaths.Length; i++)
            {
                string path = options.FrontEndRootFolderAppRelativePaths[i];

                if (string.IsNullOrWhiteSpace(path))
                {
                    i--;
                    continue;
                }

                path = path.Trim();

                if (path.StartsWith("~"))
                    path = path.Remove(0, 1);

                if (!path.StartsWith("/"))
                    path = "/" + path;

                lstCleanedPath.Add(path);
            }

            if (lstCleanedPath.Count == 0)
                throw new ArgumentException($"The cleaned items provided in {nameof(options.FrontEndRootFolderAppRelativePaths)} has resulted in an empty list.");

            options.FrontEndRootFolderAppRelativePaths = lstCleanedPath.ToArray();
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                string path = context.Request.Path.Value.Trim();

                if (Options.FrontEndRootFolderAppRelativePaths.Any(x => path.StartsWith(x, StringComparison.OrdinalIgnoreCase))
                    && Options.TargetFileExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase)
                    && context.Request.Headers.TryGetValue("accept-encoding", out string[] encodingValues))
                {
                    string flattenedEncodingHeaders = string.Join(", ", encodingValues).ToLowerInvariant();

                    CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(context.Request.CallCancelled);
                    CancellationToken token = cts.Token;

                    string physicalPath = HostingEnvironment.MapPath(path);

                    FileInfo fileInfo = new FileInfo(physicalPath);

                    if (fileInfo.Exists)
                    {
                        // Check the cache headers
                        if (context.Request.IfModifiedSinceHeaderMatched(fileInfo.LastWriteTimeUtc))
                        {
                            cts.Cancel();
                            await context.Response.SendStatusCode(HttpStatusCode.NotModified);
                            return;
                        }

                        string eTagValue = HttpHeaderValueUtility.CreateETagHeaderValue(fileInfo.LastWriteTimeUtc, fileInfo.Length);

                        if (context.Request.IfNoneMatchHeaderMatched(eTagValue))
                        {
                            cts.Cancel();
                            await context.Response.SendStatusCode(HttpStatusCode.NotModified);
                            return;
                        }

                        string cacheKey = $"{_cackeKeyPrefix}:{path}:{flattenedEncodingHeaders}";

                        var result = await Cache.GetOrCreateAsync<(string contentEncoding, byte[] bytes)>(cacheKey, async () =>
                        {
                            string contentEncoding = null;

                            using (FileStream fs = fileInfo.OpenRead())
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    if (flattenedEncodingHeaders.Contains("br", StringComparison.OrdinalIgnoreCase) || flattenedEncodingHeaders.Contains("brotli", StringComparison.OrdinalIgnoreCase))
                                    {
                                        using (BrotliStream br = new BrotliStream(ms, CompressionMode.Compress))
                                        {
                                            await fs.CopyToAsync(br, 81920, token);
                                        }

                                        contentEncoding = "br";
                                    }
                                    else if (flattenedEncodingHeaders.Contains("gzip", StringComparison.OrdinalIgnoreCase))
                                    {
                                        using (GZipStream gz = new GZipStream(ms, CompressionMode.Compress))
                                        {
                                            await fs.CopyToAsync(gz, 81920, token);
                                        }

                                        contentEncoding = "gzip";
                                    }
                                    else
                                    {
                                        await fs.CopyToAsync(ms, 81920, token);
                                    }

                                    return (contentEncoding, ms.ToArray());
                                }
                            }
                        },
                        context.Request.CallCancelled,
                        () => Options.CacheTimeout,
                        slidingExpiration: Options.CacheSlidingExpiration,
                        priority: CacheItemPriority.High,
                        expirationTokensBuilder: () => Options.WatchFiles ? new[] { new PhysicalFileChangeToken(fileInfo) } : null,
                        cacheEnabledOverride: Options.CacheEnabled);

                        await context.Response.Body.WriteAsync(result.bytes, 0, result.bytes.Length);

                        if (!string.IsNullOrEmpty(result.contentEncoding))
                            context.Response.Headers["Content-Encoding"] = result.contentEncoding;

                        context.Response.ContentType = MimeTypeUtility.GetMimeType(fileInfo.Extension);
                        context.Response.Headers["Vary"] = "Accept-Encoding";

                        context.Response.Headers["Last-Modified"] = HttpHeaderValueUtility.CreateLastModifiedHeaderValue(fileInfo.LastWriteTimeUtc);
                        context.Response.ETag = eTagValue;
                        context.Response.Expires = DateTimeOffset.UtcNow.AddYears(1);
                        context.Response.Headers["Cache-Control"] = "private, max-age=31557600, must-revalidate";

                        return;
                    }
                    else
                    {
                        cts.Cancel();
                        await context.Response.SendStatusCode(HttpStatusCode.NotFound);
                        return;
                    }
                }

                await Next.Invoke(context);
                return;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { Path = context.Request.Path.Value }))
            {
                throw;
            }
        }
    }
}