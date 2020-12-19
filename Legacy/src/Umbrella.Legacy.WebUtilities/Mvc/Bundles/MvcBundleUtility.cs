using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.WebUtilities.Bundling.Abstractions;
using Umbrella.WebUtilities.Bundling.Options;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Security;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles
{
	public class MvcBundleUtility : MvcBundleUtility<IBundleUtility, BundleUtilityOptions>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MvcBundleUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="hybridCache">The hybrid cache.</param>
		/// <param name="cacheKeyUtility">The cache key utility.</param>
		/// <param name="bundleUtility">The bundle utility.</param>
		/// <param name="options">The options.</param>
		public MvcBundleUtility(
			ILogger<MvcBundleUtility> logger,
			IHybridCache hybridCache,
			ICacheKeyUtility cacheKeyUtility,
			IBundleUtility bundleUtility,
			BundleUtilityOptions options)
			: base(logger, hybridCache, cacheKeyUtility, bundleUtility, options)
		{
		}
	}

	public abstract class MvcBundleUtility<TBundleUtility, TOptions> : IMvcBundleUtility
		where TBundleUtility : IBundleUtility
		where TOptions : BundleUtilityOptions
	{
		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger Log { get; }

		/// <summary>
		/// Gets the cache.
		/// </summary>
		protected IHybridCache Cache { get; }

		/// <summary>
		/// Gets the cache key utility.
		/// </summary>
		protected ICacheKeyUtility CacheKeyUtility { get; }

		/// <summary>
		/// Gets the bundle utility.
		/// </summary>
		protected TBundleUtility BundleUtility { get; }

		/// <summary>
		/// Gets the options.
		/// </summary>
		protected TOptions Options { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MvcBundleUtility{TBundleUtility, TOptions}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="hybridCache">The hybrid cache.</param>
		/// <param name="cacheKeyUtility">The cache key utility.</param>
		/// <param name="bundleUtility">The bundle utility.</param>
		/// <param name="options">The options.</param>
		public MvcBundleUtility(
			ILogger<MvcBundleUtility> logger,
			IHybridCache hybridCache,
			ICacheKeyUtility cacheKeyUtility,
			TBundleUtility bundleUtility,
			TOptions options)
		{
			Log = logger;
			Cache = hybridCache;
			CacheKeyUtility = cacheKeyUtility;
			BundleUtility = bundleUtility;
			Options = options;
		}

		/// <inheritdoc />
		public virtual MvcHtmlString? GetScript(string bundleName)
		{
			Guard.ArgumentNotNullOrWhiteSpace(bundleName, nameof(bundleName));

			try
			{
				string cacheKey = CacheKeyUtility.Create<MvcBundleUtility<TBundleUtility, TOptions>>($"{bundleName}:js");

				return Cache.GetOrCreate(cacheKey,
					() =>
					{
						string path = BundleUtility.GetScriptPathAsync(bundleName).Result;

						return !string.IsNullOrWhiteSpace(path) ? new MvcHtmlString($"<script src=\"{path}\"></script>") : null;
					},
					Options);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bundleName }, returnValue: true))
			{
				throw new UmbrellaWebException("There was a problem generating the script tag HTML.", exc);
			}
		}

		/// <inheritdoc />
		public virtual MvcHtmlString? GetScriptInline(string bundleName)
		{
			Guard.ArgumentNotNullOrWhiteSpace(bundleName, nameof(bundleName));

			try
			{
				string cacheKey = CacheKeyUtility.Create<MvcBundleUtility<TBundleUtility, TOptions>>($"{bundleName}:js-inline");

				return Cache.GetOrCreate(cacheKey,
					() =>
					{
						string? content = BundleUtility.GetScriptContentAsync(bundleName).Result;

						if (string.IsNullOrWhiteSpace(content))
							return null;

						string nonce = GetCurrentRequestNonce();

						var sb = new StringBuilder(!string.IsNullOrEmpty(nonce) ? $"<script defer nonce=\"{nonce}\">" : "<script defer>");
						sb.AppendLine(content);
						sb.AppendLine("</script>");

						return new MvcHtmlString(sb.ToString());
					},
					Options);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bundleName }, returnValue: true))
			{
				throw new UmbrellaWebException("There was a problem generating the inline script HTML.", exc);
			}
		}

		/// <inheritdoc />
		public virtual MvcHtmlString? GetStyleSheet(string bundleName)
		{
			Guard.ArgumentNotNullOrWhiteSpace(bundleName, nameof(bundleName));

			try
			{
				string cacheKey = CacheKeyUtility.Create<MvcBundleUtility<TBundleUtility, TOptions>>($"{bundleName}:css");

				return Cache.GetOrCreate(cacheKey,
					() =>
					{
						string path = BundleUtility.GetStyleSheetPathAsync(bundleName).Result;

						return !string.IsNullOrWhiteSpace(path) ? new MvcHtmlString($"<link rel=\"stylesheet\" href=\"{path}\" />") : null;
					},
					Options);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bundleName }, returnValue: true))
			{
				throw new UmbrellaWebException("There was a problem generating the style tag HTML.", exc);
			}
		}

		/// <inheritdoc />
		public virtual MvcHtmlString? GetStyleSheetInline(string bundleName)
		{
			Guard.ArgumentNotNullOrWhiteSpace(bundleName, nameof(bundleName));

			try
			{
				string cacheKey = CacheKeyUtility.Create<MvcBundleUtility<TBundleUtility, TOptions>>($"{bundleName}:css-inline");

				return Cache.GetOrCreate(cacheKey,
					() =>
					{
						string? content = BundleUtility.GetStyleSheetContentAsync(bundleName).Result;

						if (string.IsNullOrWhiteSpace(content))
							return null;

						string nonce = GetCurrentRequestNonce();

						var sb = new StringBuilder(!string.IsNullOrEmpty(nonce) ? $"<style nonce=\"{nonce}\">" : "<style>");
						sb.AppendLine(content);
						sb.AppendLine("</style>");

						return new MvcHtmlString(sb.ToString());
					},
					Options);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { bundleName }, returnValue: true))
			{
				throw new UmbrellaWebException("There was a problem generating the inline style HTML.", exc);
			}
		}

		/// <summary>
		/// Gets the current request nonce.
		/// </summary>
		/// <returns>The nonce value.</returns>
		protected virtual string GetCurrentRequestNonce() => HttpContext.Current.GetOwinContext().Get<string>(SecurityConstants.DefaultNonceKey);
	}
}