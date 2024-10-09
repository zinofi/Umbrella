using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.JsonArray;

/// <summary>
/// A model binder provider that can bind a JSON array to a model using the <see cref="JsonArrayModelBinder"/>.
/// </summary>
public class JsonArrayModelBinderProvider : IModelBinderProvider
{
	/// <inheritdoc />
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		Guard.IsNotNull(context);

		return context.Metadata.ModelType.IsArray
			? new JsonArrayModelBinder()
			: null;
	}
}