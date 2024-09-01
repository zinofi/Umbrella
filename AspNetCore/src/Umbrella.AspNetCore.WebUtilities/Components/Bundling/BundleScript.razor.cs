using Microsoft.AspNetCore.Components.Rendering;
using Umbrella.AspNetCore.WebUtilities.Components.Bundling.Abstractions;
using Umbrella.WebUtilities.Bundling;

namespace Umbrella.AspNetCore.WebUtilities.Components.Bundling;

public abstract class BundleScriptBase : BundleScriptComponentBase<BundleUtility>
{
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		// TODO: Execute the logic to read the file contents or get the file path.
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		base.BuildRenderTree(builder);

		// TODO: Depending on which value exists, render the script tag with the file contents or the file path.
	}
}