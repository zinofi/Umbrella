using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions;

namespace Umbrella.Legacy.WebUtilities.Mvc
{
    public abstract class UmbrellaViewPage : WebViewPage
    {
        private readonly Lazy<IBundleUtility> _bundleUtilityLazy = new Lazy<IBundleUtility>(() => DependencyResolver.Current.GetService<IBundleUtility>());

        public IBundleUtility Bundles => _bundleUtilityLazy.Value;
    }

    public abstract class UmbrellaViewPage<T> : WebViewPage<T>
    {
        private readonly Lazy<IBundleUtility> _bundleUtilityLazy = new Lazy<IBundleUtility>(() => DependencyResolver.Current.GetService<IBundleUtility>());

        public IBundleUtility Bundles => _bundleUtilityLazy.Value;
    }
}