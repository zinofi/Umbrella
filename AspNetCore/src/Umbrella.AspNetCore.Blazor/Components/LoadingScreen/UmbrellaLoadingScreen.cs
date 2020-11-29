using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Umbrella.AppFramework.Utilities.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Components.LoadingScreen
{
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
		protected override void OnInitialized()
		{
			LoadingScreenUtility.OnShow += LoadingScreenUtility_OnShow;
			LoadingScreenUtility.OnHide += LoadingScreenUtility_OnHide;
		}

		private void LoadingScreenUtility_OnShow()
		{
			_visible = true;
			StateHasChanged();
		}

		private void LoadingScreenUtility_OnHide()
		{
			_visible = false;
			StateHasChanged();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			LoadingScreenUtility.OnShow -= LoadingScreenUtility_OnShow;
			LoadingScreenUtility.OnHide -= LoadingScreenUtility_OnHide;
		}
	}
}