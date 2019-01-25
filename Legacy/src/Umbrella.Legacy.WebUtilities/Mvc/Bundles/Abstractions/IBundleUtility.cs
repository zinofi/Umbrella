using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles.Abstractions
{
    public interface IBundleUtility
    {
        MvcHtmlString GetScript(string bundleName);
        MvcHtmlString GetScriptInline(string bundleName);
        MvcHtmlString GetStyleSheet(string bundleName);
        MvcHtmlString GetStyleSheetInline(string bundleName);
    }
}