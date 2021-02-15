using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// <returns>The view as a string.</returns>
		Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
	}
}