namespace Umbrella.AspNetCore.Blazor.Components.StateViews;

/// <summary>
/// A state view component used to display a loading message.
/// </summary>
public partial class LoadingStateView
{
	/// <summary>
	/// Gets or sets the message.
	/// </summary>
	/// <remarks>
	/// Defaults to <c>Loading... Please wait.</c>
	/// </remarks>
	[Parameter]
	public string Message { get; set; } = "Loading... Please wait.";

	/// <summary>
	/// Gets or sets the content. When specified, this will override the <see cref="Message"/>.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}