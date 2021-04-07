using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.StateViews
{
	public abstract class LoadingStateViewBase : ComponentBase
	{
		[Parameter]
		public string Message { get; set; } = "Loading... Please wait.";
	}
}