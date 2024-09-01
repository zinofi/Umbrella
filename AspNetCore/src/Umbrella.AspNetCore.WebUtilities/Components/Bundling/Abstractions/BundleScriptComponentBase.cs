using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.WebUtilities.Components.Abstractions;
using Umbrella.WebUtilities.Bundling.Abstractions;
using Umbrella.WebUtilities.Security;

namespace Umbrella.AspNetCore.WebUtilities.Components.Bundling.Abstractions;

/// <summary>
/// The base class for components that render a bundle of script files.
/// </summary>
/// <typeparam name="TBundleUtility">The type of the bundle utility.</typeparam>
public abstract class BundleScriptComponentBase<TBundleUtility> : UmbrellaComponentBase
	where TBundleUtility : class, IBundleUtility
{
	/// <summary>
	/// Gets the bundle utility.
	/// </summary>
	[Inject]
	protected TBundleUtility BundleUtility { get; private set; } = null!;

	/// <summary>
	/// Gets the nonce context.
	/// </summary>
	[Inject]
	protected NonceContext NonceContext { get; private set; } = null!;

	/// <summary>
	/// Gets or sets the name of the bundle.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public string Name { get; set; } = null!;

	/// <summary>
	/// Gets or sets a value indicating whether the contents of the bundle should be rendered inline in the HTML.
	/// </summary>
	[Parameter]
	public bool RenderInline { get; set; }
}