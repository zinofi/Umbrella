using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Umbrella.AppFramework.Utilities.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions
{
	public interface IUmbrellaDialogUtility : IDialogUtility
	{
		ValueTask<bool> ShowConfirmSuccessMessageAsync(string message, string title);
		ValueTask<bool> ShowConfirmDangerMessageAsync(string message, string title);
		ValueTask<bool> ShowConfirmInfoMessageAsync(string message, string title);
		ValueTask<bool> ShowConfirmWarningMessageAsync(string message, string title);
		ValueTask<ModalResult> ShowDialogAsync(string message, string title, string cssClass, IReadOnlyCollection<UmbrellaDialogButton> buttons, string? subTitle = null);
		ValueTask<ModalResult> ShowDialogAsync<T>(string title, string cssClass, ModalParameters? modalParameters = null)
			where T : ComponentBase;
	}
}