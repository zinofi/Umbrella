using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.Messages;

/// <summary>
/// A base class used to support implementations of message components.
/// </summary>
public abstract class MessageComponentBase : ComponentBase
{
	/// <summary>
	/// The message to be displayed. If not specified, the <see cref="ChildContent"/> should be provided instead.
	/// </summary>
	/// <remarks>
	/// If a value is specified for both this property, and <see cref="ChildContent"/>, <see cref="ChildContent"/> will take precedence.
	/// </remarks>
	[Parameter]
	public string? Message { get; set; }

	/// <summary>
	/// The content to be displayed. If not specified, the <see cref="Message"/> should be provided instead.
	/// </summary>
	/// <remarks>
	/// If a value is specified for both this property, and <see cref="Message"/>, this property will take precedence.
	/// </remarks>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}