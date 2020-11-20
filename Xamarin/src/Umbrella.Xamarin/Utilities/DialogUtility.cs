using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AppFramework.Utilities.Constants;
using Umbrella.Xamarin.Exceptions;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Utilities
{
	public class DialogUtility : IDialogUtility
	{
		private readonly ILogger _logger;
		private readonly IDialogTracker _dialogTracker;

		/// <summary>
		/// Initializes a new instance of the <see cref="DialogUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dialogTracker">The dialog tracker.</param>
		public DialogUtility(
			ILogger<DialogUtility> logger,
			IDialogTracker dialogTracker)
		{
			_logger = logger;
			_dialogTracker = dialogTracker;
		}

		/// <inheritdoc />
		public async Task<bool> ShowConfirmMessageAsync(string message, string title, string acceptButtonText = "Confirm", string cancelButtonText = "Cancel")
		{
			try
			{
				return await Device.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlert(title, message, acceptButtonText, cancelButtonText));
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async Task ShowDangerMessageAsync(string message = DialogDefaults.UnknownErrorMessage, string title = "Error", string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				await Device.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlert(title, message, closeButtonText));

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async Task ShowInfoMessageAsync(string message, string title = "Information", string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				await Device.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlert(title, message, closeButtonText));

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async Task ShowMessageAsync(string message, string title, string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				await Device.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlert(title, message, closeButtonText));

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async Task ShowSuccessMessageAsync(string message, string title = "Success", string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				await Device.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlert(title, message, closeButtonText));

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async Task ShowWarningMessageAsync(string message, string title = "Warning", string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				await Device.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlert(title, message, closeButtonText));

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem showing the dialog.", exc);
			}
		}
	}
}