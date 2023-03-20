using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.Blazor.Components.StateViews;
using Umbrella.AspNetCore.Blazor.Enumerations;

namespace Umbrella.AspNetCore.Blazor.Components.ModelLayoutStateView;

/// <summary>
/// A layout state component used to simplify displaying different state views
/// based on the <see cref="LayoutState"/> value of the <see cref="CurrentState"/> property.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <seealso cref="ComponentBase" />
public partial class UmbrellaModelLayoutStateView<TModel>
{
	/// <summary>
	/// Gets or sets the state of this state view component.
	/// </summary>
	/// <remarks>
	/// Defaults to an initial value of <see cref="LayoutState.Loading"/>.
	/// A value should always be provided for this parameter.
	/// </remarks>
	[Parameter]
	[EditorRequired]
	public LayoutState CurrentState { get; set; } = LayoutState.Loading;

	/// <summary>
	/// Gets or sets the model which is used when the control has a value of
	/// <see cref="LayoutState.Success"/> for the <see cref="CurrentState"/>
	/// property.
	/// </summary>
	/// <remarks>
	/// A value should always be provided for this parameter.
	/// </remarks>
	[Parameter]
	[EditorRequired]
	public TModel? Model { get; set; }

	/// <summary>
	/// Gets or sets the callback invoked
	/// </summary>
	[Parameter]
	public EventCallback ReloadCallback { get; set; }

	/// <summary>
	/// Gets or sets the content displayed when the value of <see cref="CurrentState"/> is <see cref="LayoutState.Success"/>.
	/// </summary>
	/// <remarks>
	/// Content for this state should always be provided.
	/// </remarks>
	[Parameter]
	[EditorRequired]
	public RenderFragment<TModel>? Success { get; set; }

	/// <summary>
	/// Gets or sets the content displayed when the value of <see cref="CurrentState"/> is <see cref="LayoutState.Loading"/>.
	/// </summary>
	/// <remarks>
	/// If no content is specified, a <see cref="LoadingStateView"/> component is rendered.
	/// </remarks>
	[Parameter]
	public RenderFragment? Loading { get; set; }

	/// <summary>
	/// Gets or sets the content displayed when the value of <see cref="CurrentState"/> is <see cref="LayoutState.Error"/>.
	/// </summary>
	/// <remarks>
	/// If no content is specified, a <see cref="LoadingStateView"/> component is rendered.
	/// </remarks>
	[Parameter]
	public RenderFragment? Error { get; set; }

	/// <summary>
	/// Gets or sets the content displayed when the value of <see cref="CurrentState"/> is <see cref="LayoutState.Empty"/>.
	/// </summary>
	/// <remarks>
	/// If no content is specified, nothing is rendered.
	/// </remarks>
	[Parameter]
	public RenderFragment? Empty { get; set; }

	/// <summary>
	/// Gets or sets the content displayed when the value of <see cref="CurrentState"/> is <see cref="LayoutState.Saving"/>.
	/// </summary>
	/// <remarks>
	/// If no content is specified, nothing is rendered.
	/// </remarks>
	[Parameter]
	public RenderFragment? Saving { get; set; }
}