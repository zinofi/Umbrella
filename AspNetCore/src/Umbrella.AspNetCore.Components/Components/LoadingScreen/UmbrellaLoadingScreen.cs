using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Umbrella.AppFramework.Utilities.Abstractions;

namespace Umbrella.AspNetCore.Components.Components.LoadingScreen
{
	public class UmbrellaLoadingScreen : ComponentBase
	{
		[Inject]
		public ILoadingScreenUtility LoadingScreenUtility { get; set; } = null!;

		public bool Visible { get; set; }

		/// <inheritdoc />
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			base.BuildRenderTree(builder);

			if (Visible)
				builder.AddMarkupContent(0, "<div class=\"loading-screen\"><div class=\"loading-screen-icon\"></div></div>");
		}

		/// <inheritdoc />
		protected override void OnInitialized()
		{
			LoadingScreenUtility.OnShow += () =>
			{
				Visible = true;
				StateHasChanged();
			};

			LoadingScreenUtility.OnHide += () =>
			{
				Visible = false;
				StateHasChanged();
			};
		}
	}
}