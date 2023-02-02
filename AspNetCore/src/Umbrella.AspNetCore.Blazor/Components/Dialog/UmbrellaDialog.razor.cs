﻿using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace Umbrella.AspNetCore.Blazor.Components.Dialog;

/// <summary>
/// A dialog component that is rendered using the <see cref="BlazoredModal"/> infrastructure.
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class UmbrellaDialog
{
	[Inject]
	private NavigationManager Navigation { get; set; } = null!;

	/// <summary>
	/// Gets or sets the modal instance as a cascading parameter.
	/// </summary>
	[CascadingParameter]
	protected BlazoredModalInstance ModalInstance { get; set; } = null!;

	/// <summary>
	/// Gets or sets the size.
	/// </summary>
	/// <remarks>
	/// Defaults to <see cref="UmbrellaDialogSize.Default"/>.
	/// </remarks>
	[Parameter]
	public UmbrellaDialogSize Size { get; set; } = UmbrellaDialogSize.Default;

	/// <summary>
	/// Gets or sets a value indicating whether to show the header.
	/// </summary>
	/// <remarks>
	/// Defaults to <see langword="true"/>.
	/// </remarks>
	[Parameter]
	public bool ShowHeader { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to show the close button.
	/// </summary>
	/// <remarks>
	/// Defaults to <see langword="false"/>.
	/// </remarks>
	[Parameter]
	public bool ShowCloseButton { get; set; }

	/// <summary>
	/// Gets or sets the sub title.
	/// </summary>
	[Parameter]
	public string? SubTitle { get; set; }

	/// <summary>
	/// Gets or sets the message.
	/// </summary>
	[Parameter]
	public string? Message { get; set; }

	/// <summary>
	/// Gets or sets the buttons.
	/// </summary>
	[Parameter]
	public IReadOnlyCollection<UmbrellaDialogButton>? Buttons { get; set; }

	/// <summary>
	/// Gets or sets the custom content of the dialog.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Gets the dialog size CSS class based on the value of the <see cref="Size"/> property.
	/// </summary>
	protected string? DialogSizeCssClass => Size switch
	{
		UmbrellaDialogSize.Default => null,
		UmbrellaDialogSize.Small => "modal-sm",
		UmbrellaDialogSize.Large => "modal-lg",
		UmbrellaDialogSize.ExtraLarge => "modal-xl",
		_ => throw new SwitchExpressionException(),
	};
	
	/// <inheritdoc/>
	protected override void OnInitialized()
	{
		base.OnInitialized();

		ModalInstance.Options.HideHeader = !ShowHeader;
		ModalInstance.Options.HideCloseButton = !ShowCloseButton;
	}

	/// <summary>
	/// Handles clicks on the modal background.
	/// </summary>
	protected async Task BackgroundClickAsync()
	{
		if (ModalInstance.Options.DisableBackgroundCancel is not true)
			await ModalInstance.CancelAsync();
	}

	/// <summary>
	/// Handles close button clicks and closes the current dialog.
	/// </summary>
	protected async Task CloseClickAsync() => await ModalInstance.CancelAsync();

	/// <summary>
	/// Handles a button click of one of the specified <see cref="Buttons"/>.
	/// </summary>
	/// <param name="button">The clicked button.</param>
	protected async Task ButtonClickAsync(UmbrellaDialogButton button)
	{
		if (!string.IsNullOrWhiteSpace(button.NavigateUrl))
		{
			Navigation.NavigateTo(button.NavigateUrl);
		}
		else if (button.IsCancel)
		{
			await ModalInstance.CancelAsync();
		}
		else
		{
			await ModalInstance.CloseAsync(ModalResult.Ok(button));
		}
	}
}