using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.Dialog
{
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
		/// Handles clicks on the modal background.
		/// </summary>
		protected async Task BackgroundClick()
		{
			if (ModalInstance.Options.DisableBackgroundCancel != true)
				await ModalInstance.Cancel();
		}

		/// <summary>
		/// Handles close button clicks and closes the current dialog.
		/// </summary>
		protected async Task CloseClick() => await ModalInstance.Cancel();

		/// <summary>
		/// Handles a button click of one of the specified <see cref="Buttons"/>.
		/// </summary>
		/// <param name="button">The clicked button.</param>
		protected async Task ButtonClick(UmbrellaDialogButton button)
		{
			if (!string.IsNullOrWhiteSpace(button.NavigateUrl))
			{
				Navigation.NavigateTo(button.NavigateUrl);
			}
			else if (button.IsCancel)
			{
				await ModalInstance.Cancel();
			}
			else
			{
				await ModalInstance.Close(ModalResult.Ok(button));
			}
		}
	}
}