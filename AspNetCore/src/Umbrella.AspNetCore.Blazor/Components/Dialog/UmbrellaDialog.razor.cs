using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.Dialog
{
	public partial class UmbrellaDialog
	{
		[Inject]
		private NavigationManager Navigation { get; set; } = null!;

		[CascadingParameter]
		protected BlazoredModalInstance ModalInstance { get; set; } = null!;

		[Parameter]
		public string? SubTitle { get; set; }

		[Parameter]
		public string? Message { get; set; }

		[Parameter]
		public IReadOnlyCollection<UmbrellaDialogButton>? Buttons { get; set; }

		[Parameter]
		public RenderFragment? ChildContent { get; set; }

		protected async Task BackgroundClick()
		{
			if (ModalInstance.Options.DisableBackgroundCancel != true)
				await ModalInstance.Cancel();
		}

		protected async Task CloseClick() => await ModalInstance.Cancel();

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