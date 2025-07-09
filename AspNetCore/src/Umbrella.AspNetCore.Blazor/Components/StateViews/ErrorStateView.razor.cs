namespace Umbrella.AspNetCore.Blazor.Components.StateViews;

/// <summary>
/// A state view component used to display an error message.
/// </summary>
public partial class ErrorStateView
{
	/// <summary>
	/// Gets or sets the message.
	/// </summary>
	/// <remarks>
	/// Defaults to <c>There has been a problem. Please try again.</c>
	/// </remarks>
	[Parameter]
	public string Message { get; set; } = "There has been a problem. Please try again.";

	/// <summary>
	/// Gets or sets the content. When specified, this will override the <see cref="Message"/>.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Gets or sets the event handler that will be invoked when the reload button is clicked.
	/// </summary>
	[Parameter]
	public EventCallback OnReloadButtonClick { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to show the reload button.
	/// </summary>
	/// <remarks>
	/// Defaults to <see langword="true"/>.
	/// </remarks>
	[Parameter]
	public bool ShowReloadButton { get; set; } = true;

	/// <summary>
	/// Gets or sets the reload button text.
	/// </summary>
	/// <remarks>Defaults to <c>Reload</c></remarks>
	[Parameter]
	public string ReloadButtonText { get; set; } = "Reload";
}