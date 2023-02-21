using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.Blazor.Enumerations;

namespace Umbrella.AspNetCore.Blazor.Components.FormLayoutStateView;

public partial class UmbrellaFormLayoutStateView
{
	[Parameter]
	[EditorRequired]
	public LayoutState CurrentState { get; set; } = LayoutState.Loading;

	[Parameter]
	public object? Model { get; set; }

	[Parameter]
	public EventCallback ReloadCallback { get; set; }

	[Parameter]
	[EditorRequired]
	public RenderFragment? Success { get; set; }

	[Parameter]
	public RenderFragment? Loading { get; set; }

	[Parameter]
	public RenderFragment? Error { get; set; }
}