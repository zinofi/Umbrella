﻿using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Umbrella.AspNetCore.Shared.Components.Abstractions;
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
	private string? _contentOrPath;
	private bool _shouldRender = true;

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

		_contentOrPath = RenderInline ? await BundleUtility.GetScriptContentAsync(Name) : await BundleUtility.GetScriptPathAsync(Name);
	}

	/// <inheritdoc/>
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		Guard.IsNotNull(builder);

		base.BuildRenderTree(builder);

		if (string.IsNullOrEmpty(_contentOrPath))
			return;

		builder.OpenElement(0, "script");

		if (RenderInline)
		{
			builder.AddAttribute(1, "nonce", NonceContext.Current);
			builder.AddContent(2, _contentOrPath);
		}
		else
		{
			builder.AddAttribute(1, "src", _contentOrPath);
		}

		builder.CloseElement();

		_shouldRender = false;
	}

	/// <inheritdoc/>
	protected override bool ShouldRender() => _shouldRender;
}