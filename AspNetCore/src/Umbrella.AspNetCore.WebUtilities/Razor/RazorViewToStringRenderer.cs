using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Umbrella.AspNetCore.WebUtilities.Razor.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Razor
{
	/// <summary>
	/// A utility used to render a view to a string.
	/// </summary>
	/// <seealso cref="IRazorViewToStringRenderer" />
	public class RazorViewToStringRenderer : IRazorViewToStringRenderer
	{
		private readonly IRazorViewEngine _viewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IHttpContextAccessor _httpContextAccessor;

		/// <summary>
		/// Initializes a new instance of the <see cref="RazorViewToStringRenderer"/> class.
		/// </summary>
		/// <param name="viewEngine">The view engine.</param>
		/// <param name="tempDataProvider">The temp data provider.</param>
		/// <param name="httpContextAccessor">The HTTP context accessor.</param>
		public RazorViewToStringRenderer(
			IRazorViewEngine viewEngine,
			ITempDataProvider tempDataProvider,
			IHttpContextAccessor httpContextAccessor)
		{
			_viewEngine = viewEngine;
			_tempDataProvider = tempDataProvider;
			_httpContextAccessor = httpContextAccessor;
		}

		/// <inheritdoc />
		public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
		{
			var actionContext = new ActionContext(_httpContextAccessor.HttpContext, new RouteData(), new ActionDescriptor());
			var view = FindView(actionContext, viewName);

			using (var output = new StringWriter())
			{
				var viewContext = new ViewContext(
					actionContext,
					view,
					new ViewDataDictionary<TModel>(
						metadataProvider: new EmptyModelMetadataProvider(),
						modelState: new ModelStateDictionary())
					{
						Model = model
					},
					new TempDataDictionary(
						actionContext.HttpContext,
						_tempDataProvider),
					output,
					new HtmlHelperOptions());

				await view.RenderAsync(viewContext);

				return output.ToString();
			}
		}

		private IView FindView(ActionContext actionContext, string viewName)
		{
			var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
			if (getViewResult.Success)
			{
				return getViewResult.View;
			}

			var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
			if (findViewResult.Success)
			{
				return findViewResult.View;
			}

			var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
			string? errorMessage = string.Join(
				Environment.NewLine,
				new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

			throw new InvalidOperationException(errorMessage);
		}
	}
}
