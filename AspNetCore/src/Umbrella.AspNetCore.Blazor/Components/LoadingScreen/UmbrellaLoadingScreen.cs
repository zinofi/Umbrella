// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AppFramework.Utilities.Enumerations;

namespace Umbrella.AspNetCore.Blazor.Components.LoadingScreen;

/// <summary>
/// A component that is used to show and hide a loading screen in response to events fired by the <see cref="ILoadingScreenUtility"/> service.
/// </summary>
/// <seealso cref="ComponentBase" />
/// <seealso cref="IDisposable" />
public class UmbrellaLoadingScreen : ComponentBase, IDisposable
{
	private bool _visible;

	[Inject]
	private ILoadingScreenUtility LoadingScreenUtility { get; set; } = null!;

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		base.BuildRenderTree(builder);

		if (_visible)
			builder.AddMarkupContent(0, "<div class=\"loading-screen\"><div class=\"loading-screen-icon\"></div></div>");
	}

	/// <inheritdoc />
	protected override void OnInitialized() => LoadingScreenUtility.OnStateChanged += LoadingScreenUtility_OnStateChanged;

	private void LoadingScreenUtility_OnStateChanged(LoadingScreenState state)
	{
		_visible = state switch
		{
			LoadingScreenState.Visible => true,
			LoadingScreenState.Hidden => false,
			_ => false,
		};

		StateHasChanged();
	}

	/// <inheritdoc />
	public void Dispose() => LoadingScreenUtility.OnStateChanged -= LoadingScreenUtility_OnStateChanged;
}