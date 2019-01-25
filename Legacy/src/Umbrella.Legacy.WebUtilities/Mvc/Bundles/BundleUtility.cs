using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Options;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles
{
    public class BundleUtility : IBundleUtility
    {
        protected ILogger Log { get; }
        protected BundleUtilityOptions Options { get; }
        protected IMultiCache Cache { get; }
        protected IUmbrellaWebHostingEnvironment HostingEnvironment { get; }

        public BundleUtility(
            ILogger<BundleUtility> logger,
            BundleUtilityOptions options,
            IMultiCache multiCache,
            IUmbrellaWebHostingEnvironment hostingEnvironment)
        {
            Log = logger;
            Options = options;
            Cache = multiCache;
            HostingEnvironment = hostingEnvironment;

            Guard.ArgumentNotNullOrWhiteSpace(Options.DefaultBundleFolderAppRelativePath, "The default path to the bundles must be specified.");

            // Ensure the path ends with a slash
            if (!Options.DefaultBundleFolderAppRelativePath.EndsWith("/"))
                Options.DefaultBundleFolderAppRelativePath += "/";
        }

        public MvcHtmlString GetScript(string bundleNameOrPath)
        {
            Guard.ArgumentNotNullOrWhiteSpace(bundleNameOrPath, nameof(bundleNameOrPath));

            try
            {
                return BuildOutput(bundleNameOrPath, () =>
                {
                    string path = ResolveBundlePath(bundleNameOrPath, "js");

                    var builder = new TagBuilder("script");
                    builder.MergeAttribute("defer", "defer");
                    builder.MergeAttribute("src", path);

                    return MvcHtmlString.Create(builder.ToString());
                });
            }
            catch (Exception exc) when (Log.WriteError(exc, new { bundleNameOrPath }))
            {
                throw;
            }
        }

        public MvcHtmlString GetScriptInline(string bundleNameOrPath)
        {
            Guard.ArgumentNotNullOrWhiteSpace(bundleNameOrPath, nameof(bundleNameOrPath));

            try
            {
                return BuildOutput(bundleNameOrPath, () =>
                {
                    string content = ResolveBundleContent(bundleNameOrPath, "js");

                    var builder = new TagBuilder("script")
                    {
                        InnerHtml = content
                    };

                    return MvcHtmlString.Create(builder.ToString());
                });
            }
            catch (Exception exc) when (Log.WriteError(exc, new { bundleNameOrPath }))
            {
                throw;
            }
        }

        public MvcHtmlString GetStyleSheet(string bundleNameOrPath)
        {
            Guard.ArgumentNotNullOrWhiteSpace(bundleNameOrPath, nameof(bundleNameOrPath));

            try
            {
                return BuildOutput(bundleNameOrPath, () =>
                {
                    string path = ResolveBundlePath(bundleNameOrPath, "css");

                    var builder = new TagBuilder("link");
                    builder.MergeAttribute("rel", "stylesheet");
                    builder.MergeAttribute("href", path);

                    return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
                });
            }
            catch (Exception exc) when (Log.WriteError(exc, new { bundleNameOrPath }))
            {
                throw;
            }
        }

        public MvcHtmlString GetStyleSheetInline(string bundleNameOrPath)
        {
            Guard.ArgumentNotNullOrWhiteSpace(bundleNameOrPath, nameof(bundleNameOrPath));

            try
            {
                return BuildOutput(bundleNameOrPath, () =>
                {
                    string content = ResolveBundleContent(bundleNameOrPath, "css");

                    var builder = new TagBuilder("style")
                    {
                        InnerHtml = content
                    };

                    return MvcHtmlString.Create(builder.ToString());
                });
            }
            catch (Exception exc) when (Log.WriteError(exc, new { bundleNameOrPath }))
            {
                throw;
            }
        }

        private MvcHtmlString BuildOutput(string bundleNameOrPath, Func<MvcHtmlString> builder)
        {
            // When watching the source files, we can't cache the generated HTML string here and need to rebuild it everytime.
            return Options.WatchFiles
                ? builder()
                : Cache.GetOrCreate(bundleNameOrPath,
                () => builder(),
                () => Options.CacheTimeout,
                slidingExpiration: Options.CacheSlidingExpiration,
                priority: CacheItemPriority.NeverRemove,
                cacheEnabledOverride: Options.CacheEnabled);
        }

        private string ResolveBundlePath(string bundleNameOrPath, string bundleType)
            => HostingEnvironment.MapWebPath($"{DetermineBundleName(bundleNameOrPath)}.{bundleType}", appendVersion: true, watchWhenAppendVersion: Options.WatchFiles);

        private string ResolveBundleContent(string bundleNameOrPath, string bundleType)
            => HostingEnvironment.GetFileContent($"{DetermineBundleName(bundleNameOrPath)}.{bundleType}", Options.CacheEnabled, Options.WatchFiles);

        private string DetermineBundleName(string bundleNameOrPath)
        {
            if (bundleNameOrPath.StartsWith("~") || bundleNameOrPath.StartsWith("/"))
                return bundleNameOrPath;

            return Options.DefaultBundleFolderAppRelativePath + bundleNameOrPath;
        }
    }
}