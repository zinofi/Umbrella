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
		Task<bool> ShowConfirmSuccessMessageAsync(string message, string title);
		Task<bool> ShowConfirmDangerMessageAsync(string message, string title);
		Task<bool> ShowConfirmInfoMessageAsync(string message, string title);
		Task<bool> ShowConfirmWarningMessageAsync(string message, string title);
		Task<ModalResult> ShowDialogAsync(string message, string title, string cssClass, IReadOnlyCollection<UmbrellaDialogButton> buttons, string? subTitle = null);
		Task<ModalResult> ShowDialogAsync<T>(string title, string cssClass, ModalParameters? modalParameters = null)
			where T : ComponentBase;
	}
}