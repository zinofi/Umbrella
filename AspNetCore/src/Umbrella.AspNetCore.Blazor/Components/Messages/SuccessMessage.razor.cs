using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.Messages;

public abstract class SuccessMessageBase : ComponentBase
{
	[Parameter]
	public string? Message { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}