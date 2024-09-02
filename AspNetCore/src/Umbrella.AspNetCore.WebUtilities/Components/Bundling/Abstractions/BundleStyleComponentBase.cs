using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.WebUtilities.Components.Abstractions;
using Umbrella.WebUtilities.Bundling.Abstractions;
using Umbrella.WebUtilities.Security;
using CommunityToolkit.Diagnostics;

namespace Umbrella.AspNetCore.WebUtilities.Components.Bundling.Abstractions;

/// <summary>
/// A base class for components that render a bundle of style files.
/// </summary>
/// <typeparam name="TBundleUtility">The type of the bundle utility.</typeparam>
public abstract class BundleStyleComponentBase<TBundleUtility> : UmbrellaComponentBase
	where TBundleUtility : class, IBundleUtility
{
	private string? _contentOrPath;

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

	/// <inheritdoc/>
	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		Guard.IsNotNullOrWhiteSpace(Name);
		Name = Name.TrimToLowerInvariant();
		Guard.IsNotNullOrWhiteSpace(Name);
	}

	/// <inheritdoc/>
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		_contentOrPath = RenderInline ? await BundleUtility.GetStyleSheetContentAsync(Name) : await BundleUtility.GetStyleSheetPathAsync(Name);
	}

	/// <inheritdoc/>
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		Guard.IsNotNull(builder);

		base.BuildRenderTree(builder);

		if (string.IsNullOrEmpty(_contentOrPath))
			throw new InvalidOperationException("The content or path does not exist.");

		if (RenderInline)
		{
			builder.OpenElement(0, "style");
			builder.AddAttribute(1, "nonce", NonceContext.Current);
			builder.AddContent(2, _contentOrPath);
		}
		else
		{
			builder.OpenElement(0, "link");
			builder.AddAttribute(1, "rel", "stylesheet");
			builder.AddAttribute(2, "href", _contentOrPath);
		}

		builder.CloseElement();
	}
}