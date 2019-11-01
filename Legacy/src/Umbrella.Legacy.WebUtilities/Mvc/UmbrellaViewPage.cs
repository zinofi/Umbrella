using System;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;

namespace Umbrella.Legacy.WebUtilities.Mvc
{
	public abstract class UmbrellaViewPage : WebViewPage
	{
		private readonly Lazy<IMvcBundleUtility> _bundleUtilityLazy = new Lazy<IMvcBundleUtility>(() => DependencyResolver.Current.GetService<IMvcBundleUtility>());
		private readonly Lazy<IMvcWebpackBundleUtility> _webpackBundleUtilityLazy = new Lazy<IMvcWebpackBundleUtility>(() => DependencyResolver.Current.GetService<IMvcWebpackBundleUtility>());

		public IMvcBundleUtility Bundles => _bundleUtilityLazy.Value;
		public IMvcWebpackBundleUtility WebpackBundles => _webpackBundleUtilityLazy.Value;

	}

	public abstract class UmbrellaViewPage<T> : WebViewPage<T>
	{
		private readonly Lazy<IMvcBundleUtility> _bundleUtilityLazy = new Lazy<IMvcBundleUtility>(() => DependencyResolver.Current.GetService<IMvcBundleUtility>());
		private readonly Lazy<IMvcWebpackBundleUtility> _webpackBundleUtilityLazy = new Lazy<IMvcWebpackBundleUtility>(() => DependencyResolver.Current.GetService<IMvcWebpackBundleUtility>());

		public IMvcBundleUtility Bundles => _bundleUtilityLazy.Value;
		public IMvcWebpackBundleUtility WebpackBundles => _webpackBundleUtilityLazy.Value;
	}
}