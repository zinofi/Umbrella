using System.Threading.Tasks;
using Umbrella.AppFramework.Utilities.Constants;

namespace Umbrella.AppFramework.Utilities.Abstractions
{
	public interface IDialogUtility
	{
		Task ShowMessageAsync(string message, string title, string closeButtonText = DialogDefaults.DefaultCloseButtonText);
		Task ShowDangerMessageAsync(string message = DialogDefaults.UnknownErrorMessage, string title = "Error", string closeButtonText = DialogDefaults.DefaultCloseButtonText);
		Task ShowSuccessMessageAsync(string message, string title = "Success", string closeButtonText = DialogDefaults.DefaultCloseButtonText);
		Task ShowInfoMessageAsync(string message, string title = "Information", string closeButtonText = DialogDefaults.DefaultCloseButtonText);
		Task ShowWarningMessageAsync(string message, string title = "Warning", string closeButtonText = DialogDefaults.DefaultCloseButtonText);
		Task<bool> ShowConfirmMessageAsync(string message, string title, string acceptButtonText = DialogDefaults.DefaultConfirmButtonText, string cancelButtonText = DialogDefaults.DefaultCancelButtonText);
	}
}