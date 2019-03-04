using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Options;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Primitives;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles
{
	public class WebpackBundleUtility : BundleUtility<WebpackBundleUtilityOptions>, IWebpackBundleUtility
	{
		private readonly string _manifestJsonFileSubPath;

		protected ICacheKeyUtility CacheKeyUtility { get; }
		protected internal IFileProvider FileProvider { get; internal set; }

		public WebpackBundleUtility(
			ILogger<WebpackBundleUtility> logger,
			WebpackBundleUtilityOptions options,
			IMultiCache multiCache,
			IUmbrellaWebHostingEnvironment hostingEnvironment,
			ICacheKeyUtility cacheKeyUtility)
			: base(logger, options, multiCache, hostingEnvironment)
		{
			CacheKeyUtility = cacheKeyUtility;

			string rootPath = hostingEnvironment.MapPath(Options.DefaultBundleFolderAppRelativePath);
			FileProvider = new PhysicalFileProvider(rootPath);

			Guard.ArgumentNotNullOrWhiteSpace(Options.ManifestJsonFileName, nameof(Options.ManifestJsonFileName));
			Options.ManifestJsonFileName = Options.ManifestJsonFileName.Trim().Trim('/').ToLowerInvariant();

			string extension = Path.GetExtension(Options.ManifestJsonFileName);

			if (!string.IsNullOrWhiteSpace(extension) && !extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException("The manifest file name must only have an extenion of .json.");

			if (string.IsNullOrWhiteSpace(extension))
				Options.ManifestJsonFileName = Options.ManifestJsonFileName + ".json";

			_manifestJsonFileSubPath = "/" + Options.ManifestJsonFileName;
		}

		public override MvcHtmlString GetScript(string bundleNameOrPath)
		{
			Guard.ArgumentNotNullOrWhiteSpace(bundleNameOrPath, nameof(bundleNameOrPath));

			try
			{
				return GetScript(bundleNameOrPath, false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bundleNameOrPath }, returnValue: true))
			{
				throw new UmbrellaWebException("There has been a problem creating the Webpack HTML script tag.", exc);
			}
		}

		public override MvcHtmlString GetStyleSheet(string bundleNameOrPath)
		{
			Guard.ArgumentNotNullOrWhiteSpace(bundleNameOrPath, nameof(bundleNameOrPath));

			try
			{
				return GetStyleSheet(bundleNameOrPath, false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bundleNameOrPath }, returnValue: true))
			{
				throw new UmbrellaWebException("There has been a problem creating the Webpack HTML style tag.", exc);
			}
		}

		protected override string DetermineBundlePath(string bundleNameOrPath, string bundleType)
		{
			if (bundleNameOrPath.StartsWith("~") || bundleNameOrPath.StartsWith("/"))
				throw new ArgumentException("The path must not be app relative. It should just be the webpack bundle name.");

			if (Path.HasExtension(bundleNameOrPath))
				bundleNameOrPath = Path.GetFileNameWithoutExtension(bundleNameOrPath);

			string bundleNameKey = (bundleNameOrPath + "." + bundleType).ToLowerInvariant();

			Dictionary<string, string> dicManifest = GetManifest();

			if (dicManifest.TryGetValue(bundleNameKey, out string bundlePath))
				return bundlePath;

			throw new Exception("The specified Webpack bundle cannot be found.");
		}

		private Dictionary<string, string> GetManifest()
		{
			try
			{
				string cacheKey = CacheKeyUtility.Create<WebpackBundleUtility>("WebpackManifest");

				return Cache.GetOrCreate(cacheKey, () =>
				{
					IFileInfo fileInfo = FileProvider.GetFileInfo(_manifestJsonFileSubPath);

					if (!fileInfo.Exists)
						throw new Exception("The specified Webpack Manifest JSON file does not exist.");

					using (Stream stream = fileInfo.CreateReadStream())
					{
						using (var sr = new StreamReader(stream))
						{
							string json = sr.ReadToEnd();

							return UmbrellaStatics.DeserializeJson<Dictionary<string, string>>(json).ToDictionary(x => x.Key.ToLowerInvariant(), x =>
							{
								string loweredPath = x.Value.ToLowerInvariant();

								int idxLastSlash = loweredPath.LastIndexOf('/');

								if(idxLastSlash != -1)
									loweredPath = loweredPath.Substring(idxLastSlash + 1);

								return Path.Combine(Options.DefaultBundleFolderAppRelativePath, loweredPath);
							});
						}
					}
				},
				() => Options.CacheTimeout,
				slidingExpiration: Options.CacheSlidingExpiration,
				priority: CacheItemPriority.NeverRemove,
				expirationTokensBuilder: () => Options.WatchFiles ? new[] { FileProvider.Watch(_manifestJsonFileSubPath) } : null,
				cacheEnabledOverride: Options.CacheEnabled);
			}
			catch (Exception exc)
			{
				throw new Exception("There has been a problem reading the Webpack Manifest JSON.", exc);
			}
		}
	}
}