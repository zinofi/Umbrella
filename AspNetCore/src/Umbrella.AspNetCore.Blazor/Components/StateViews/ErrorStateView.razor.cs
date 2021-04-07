using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.StateViews
{
	public abstract class ErrorStateViewBase : ComponentBase
	{
		[Parameter]
		public string Message { get; set; } = "There has been a problem. Please try again.";

		[Parameter]
		public EventCallback OnReloadButtonClick { get; set; }
	}
}