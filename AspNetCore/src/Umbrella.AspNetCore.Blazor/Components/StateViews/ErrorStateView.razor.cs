using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.StateViews;

/// <summary>
/// A state view component used to display an error message. Defaults to "There has been a problem. Please try again.".
/// </summary>
public partial class ErrorStateView
{
	/// <summary>
	/// Gets or sets the message. Defaults to "There has been a problem. Please try again.".
	/// </summary>
	[Parameter]
	public string Message { get; set; } = "There has been a problem. Please try again.";

	/// <summary>
	/// Gets or sets the event handler that will be invoked when the reload button is clicked.
	/// </summary>
	[Parameter]
	public EventCallback OnReloadButtonClick { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to show the reload button. Defaults to <see langword="true"/>.
	/// </summary>
	[Parameter]
	public bool ShowReloadButton { get; set; } = true;
}