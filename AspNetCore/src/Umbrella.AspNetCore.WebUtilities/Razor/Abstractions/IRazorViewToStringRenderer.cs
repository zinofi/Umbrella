using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.Razor.Abstractions
{
	/// <summary>
	/// A utility used to render a view to a string.
	/// </summary>
	public interface IRazorViewToStringRenderer
    {
		/// <summary>
		/// Renders the MVC View with the specified <paramref name="viewName"/> using the specified <paramref name="model"/>
		/// to a string.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <param name="viewName">The view name.</param>
		/// <param name="model">The model.</param>
		/// <param name="httpContext">
		/// The <see cref="HttpContext"/>. If this is not specified, the HttpContext for the currently executing request will be used.
		/// Allowing this to be specified is useful in cases where the View is being rendered outside of a HTTP request,
		/// e.g. when executed from an <see cref="IHostedService"/>, meaning there is no ambient HTTPContext.
		/// </param>
		/// <returns>The view as a string.</returns>
		Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, HttpContext? httpContext = null);
	}
}