using System;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;

namespace Umbrella.Legacy.WebUtilities.Mvc
{
	/// <summary>
	/// Serves as the base class <see cref="WebViewPage"/> instances and provides access to <see cref="Bundles"/> and <see cref="WebpackBundles"/> inside Razor views
	/// that use this base class.
	/// </summary>
	/// <seealso cref="WebViewPage" />
	public abstract class UmbrellaViewPage : WebViewPage
	{
		private readonly Lazy<IMvcBundleUtility> _bundleUtilityLazy = new Lazy<IMvcBundleUtility>(() => DependencyResolver.Current.GetService<IMvcBundleUtility>());
		private readonly Lazy<IMvcWebpackBundleUtility> _webpackBundleUtilityLazy = new Lazy<IMvcWebpackBundleUtility>(() => DependencyResolver.Current.GetService<IMvcWebpackBundleUtility>());

		/// <summary>
		/// Gets the bundles.
		/// </summary>
		public IMvcBundleUtility Bundles => _bundleUtilityLazy.Value;

		/// <summary>
		/// Gets the webpack bundles.
		/// </summary>
		public IMvcWebpackBundleUtility WebpackBundles => _webpackBundleUtilityLazy.Value;
	}

	/// <summary>
	/// Serves as the base class <see cref="WebViewPage"/> instances and provides access to <see cref="Bundles"/> and <see cref="WebpackBundles"/> inside Razor views
	/// that use this base class.
	/// </summary>
	/// <typeparam name="T">The type of the model.</typeparam>
	/// <seealso cref="System.Web.Mvc.WebViewPage" />
	public abstract class UmbrellaViewPage<T> : WebViewPage<T>
	{
		private readonly Lazy<IMvcBundleUtility> _bundleUtilityLazy = new Lazy<IMvcBundleUtility>(() => DependencyResolver.Current.GetService<IMvcBundleUtility>());
		private readonly Lazy<IMvcWebpackBundleUtility> _webpackBundleUtilityLazy = new Lazy<IMvcWebpackBundleUtility>(() => DependencyResolver.Current.GetService<IMvcWebpackBundleUtility>());

		/// <summary>
		/// Gets the bundles.
		/// </summary>
		public IMvcBundleUtility Bundles => _bundleUtilityLazy.Value;

		/// <summary>
		/// Gets the webpack bundles.
		/// </summary>
		public IMvcWebpackBundleUtility WebpackBundles => _webpackBundleUtilityLazy.Value;
	}
}