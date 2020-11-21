using System.Threading.Tasks;
using Umbrella.AppFramework.Utilities.Constants;

namespace Umbrella.AppFramework.Utilities.Abstractions
{
	/// <summary>
	/// A utility used to show application dialogs.
	/// </summary>
	public interface IDialogUtility
	{
		/// <summary>
		/// Shows a dialog with a message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="title">The title.</param>
		/// <param name="closeButtonText">The close button text.</param>
		/// <returns>An awaitable task that completes when the dialog has been actioned.</returns>
		ValueTask ShowMessageAsync(string message, string title, string closeButtonText = DialogDefaults.DefaultCloseButtonText);

		/// <summary>
		/// Shows a dialog with a message indicating danger.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="title">The title.</param>
		/// <param name="closeButtonText">The close button text.</param>
		/// <returns>An awaitable task that completes when the dialog has been actioned.</returns>
		ValueTask ShowDangerMessageAsync(string message = DialogDefaults.UnknownErrorMessage, string title = "Error", string closeButtonText = DialogDefaults.DefaultCloseButtonText);

		/// <summary>
		/// Shows a dialog with a message indicating success.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="title">The title.</param>
		/// <param name="closeButtonText">The close button text.</param>
		/// <returns>An awaitable task that completes when the dialog has been actioned.</returns>
		ValueTask ShowSuccessMessageAsync(string message, string title = "Success", string closeButtonText = DialogDefaults.DefaultCloseButtonText);

		/// <summary>
		/// Shows a dialog with a message indicating information.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="title">The title.</param>
		/// <param name="closeButtonText">The close button text.</param>
		/// <returns>An awaitable task that completes when the dialog has been actioned.</returns>
		ValueTask ShowInfoMessageAsync(string message, string title = "Information", string closeButtonText = DialogDefaults.DefaultCloseButtonText);

		/// <summary>
		/// Shows a dialog with a message indicating warning.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="title">The title.</param>
		/// <param name="closeButtonText">The close button text.</param>
		/// <returns>An awaitable task that completes when the dialog has been actioned.</returns>
		ValueTask ShowWarningMessageAsync(string message, string title = "Warning", string closeButtonText = DialogDefaults.DefaultCloseButtonText);


		/// <summary>
		/// Shows a confirmation dialog.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="title">The title.</param>
		/// <param name="acceptButtonText">The accept button text.</param>
		/// <param name="cancelButtonText">The cancel button text.</param>
		/// <returns>An awaitable task that completes when the dialog has been actioned with a value indicating whether the message was confirmed or not by the user.</returns>
		ValueTask<bool> ShowConfirmMessageAsync(string message, string title, string acceptButtonText = DialogDefaults.DefaultConfirmButtonText, string cancelButtonText = DialogDefaults.DefaultCancelButtonText);
	}
}