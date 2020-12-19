using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AppFramework.Utilities.Constants;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Exceptions;

namespace Umbrella.AspNetCore.Blazor.Components.Dialog
{
	/// <summary>
	/// Used to show dialogs in Blazor applications.
	/// </summary>
	/// <seealso cref="IUmbrellaDialogUtility" />
	public class UmbrellaDialogUtility : IUmbrellaDialogUtility
	{
		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultMessageButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCloseButtonText, UmbrellaDialogButtonType.Primary)
		};

		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultSuccessMessageButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCloseButtonText, UmbrellaDialogButtonType.Success)
		};

		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultDangerMessageButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCloseButtonText, UmbrellaDialogButtonType.Danger)
		};

		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultInfoMessageButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCloseButtonText, UmbrellaDialogButtonType.Info)
		};

		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultWarningMessageButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCloseButtonText, UmbrellaDialogButtonType.Warning)
		};

		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultConfirmButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCancelButtonText, UmbrellaDialogButtonType.Default, true),
			new UmbrellaDialogButton(DialogDefaults.DefaultConfirmButtonText, UmbrellaDialogButtonType.Primary)
		};

		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultConfirmSuccessButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCancelButtonText, UmbrellaDialogButtonType.Default, true),
			new UmbrellaDialogButton(DialogDefaults.DefaultConfirmButtonText, UmbrellaDialogButtonType.Success)
		};

		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultConfirmDangerButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCancelButtonText, UmbrellaDialogButtonType.Default, true),
			new UmbrellaDialogButton(DialogDefaults.DefaultConfirmButtonText, UmbrellaDialogButtonType.Danger)
		};

		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultConfirmInfoButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCancelButtonText, UmbrellaDialogButtonType.Default, true),
			new UmbrellaDialogButton(DialogDefaults.DefaultConfirmButtonText, UmbrellaDialogButtonType.Info)
		};

		private readonly IReadOnlyCollection<UmbrellaDialogButton> _defaultConfirmWarningButtons = new[]
		{
			new UmbrellaDialogButton(DialogDefaults.DefaultCancelButtonText, UmbrellaDialogButtonType.Default, true),
			new UmbrellaDialogButton(DialogDefaults.DefaultConfirmButtonText, UmbrellaDialogButtonType.Warning)
		};

		private readonly ILogger _logger;
		private readonly IDialogTracker _dialogTracker;
		private readonly IModalService _modalService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaDialogUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dialogTracker">The dialog tracker.</param>
		/// <param name="modalService">The modal service.</param>
		public UmbrellaDialogUtility(
			ILogger<UmbrellaDialogUtility> logger,
			IDialogTracker dialogTracker,
			IModalService modalService)
		{
			_logger = logger;
			_dialogTracker = dialogTracker;
			_modalService = modalService;
		}

		/// <inheritdoc />
		public async ValueTask ShowMessageAsync(string message, string title, string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				var buttons = closeButtonText is DialogDefaults.DefaultCloseButtonText
					? _defaultMessageButtons
					: new[] { new UmbrellaDialogButton(closeButtonText, UmbrellaDialogButtonType.Primary) };

				await ShowDialogAsync(message, title, "u-dialog--message", buttons);

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask ShowDangerMessageAsync(string message = "An unknown error has occurred. Please try again. If problems persist, please reload the screen.", string title = "Error", string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				var buttons = closeButtonText is DialogDefaults.DefaultCloseButtonText
					? _defaultDangerMessageButtons
					: new[] { new UmbrellaDialogButton(closeButtonText, UmbrellaDialogButtonType.Danger) };

				await ShowDialogAsync(message, title, "u-dialog--message u-dialog--message-danger", buttons);

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask ShowInfoMessageAsync(string message, string title = "Information", string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				var buttons = closeButtonText is DialogDefaults.DefaultCloseButtonText
					? _defaultInfoMessageButtons
					: new[] { new UmbrellaDialogButton(closeButtonText, UmbrellaDialogButtonType.Info) };

				_logger.WriteInformation(message: "Opening info dialog");

				await ShowDialogAsync(message, title, "u-dialog--message u-dialog--message-info", buttons);

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask ShowSuccessMessageAsync(string message, string title = "Success", string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				var buttons = closeButtonText is DialogDefaults.DefaultCloseButtonText
					? _defaultSuccessMessageButtons
					: new[] { new UmbrellaDialogButton(closeButtonText, UmbrellaDialogButtonType.Success) };

				await ShowDialogAsync(message, title, "u-dialog--message u-dialog--message-success", buttons);

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask ShowWarningMessageAsync(string message, string title = "Warning", string closeButtonText = "Close")
		{
			try
			{
				int code = _dialogTracker.GenerateCode(message, title, null, closeButtonText);

				if (!_dialogTracker.TrackOpen(code))
					return;

				var buttons = closeButtonText is DialogDefaults.DefaultCloseButtonText
					? _defaultWarningMessageButtons
					: new[] { new UmbrellaDialogButton(closeButtonText, UmbrellaDialogButtonType.Warning) };

				await ShowDialogAsync(message, title, "u-dialog--message u-dialog--message-warning", buttons);

				_dialogTracker.Close(code);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask<bool> ShowConfirmMessageAsync(string message, string title, string acceptButtonText = "Confirm", string cancelButtonText = "Cancel")
		{
			try
			{
				var buttons = acceptButtonText is DialogDefaults.DefaultConfirmButtonText && cancelButtonText is DialogDefaults.DefaultCancelButtonText
					? _defaultConfirmButtons
					: new[]
					{
						new UmbrellaDialogButton(cancelButtonText, UmbrellaDialogButtonType.Danger),
						new UmbrellaDialogButton(acceptButtonText, UmbrellaDialogButtonType.Danger)
					};

				ModalResult result = await ShowDialogAsync(message, title, "u-dialog--confirm", buttons);

				return !result.Cancelled;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask<bool> ShowConfirmDangerMessageAsync(string message, string title)
		{
			try
			{
				ModalResult result = await ShowDialogAsync(message, title, "u-dialog--confirm u-dialog--confirm-danger", _defaultConfirmDangerButtons);

				return !result.Cancelled;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask<bool> ShowConfirmSuccessMessageAsync(string message, string title)
		{
			try
			{
				ModalResult result = await ShowDialogAsync(message, title, "u-dialog--confirm u-dialog--confirm-success", _defaultConfirmSuccessButtons);

				return !result.Cancelled;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask<bool> ShowConfirmInfoMessageAsync(string message, string title)
		{
			try
			{
				ModalResult result = await ShowDialogAsync(message, title, "u-dialog--confirm u-dialog--confirm-info", _defaultConfirmInfoButtons);

				return !result.Cancelled;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask<bool> ShowConfirmWarningMessageAsync(string message, string title)
		{
			try
			{
				ModalResult result = await ShowDialogAsync(message, title, "u-dialog--confirm u-dialog--confirm-warning", _defaultConfirmWarningButtons);

				return !result.Cancelled;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask<ModalResult> ShowDialogAsync(string message, string title, string cssClass, IReadOnlyCollection<UmbrellaDialogButton> buttons, string? subTitle = null)
		{
			try
			{
				var parameters = new ModalParameters();
				parameters.Add(nameof(UmbrellaDialog.SubTitle), subTitle);
				parameters.Add(nameof(UmbrellaDialog.Message), message);
				parameters.Add(nameof(UmbrellaDialog.Buttons), buttons);

				var options = new ModalOptions
				{
					Class = cssClass,
					DisableBackgroundCancel = true,
					UseCustomLayout = true
				};

				IModalReference modal = _modalService.Show<UmbrellaDialog>(title, parameters, options);

				return await modal.Result;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { message, title, cssClass, buttons, subTitle }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask<ModalResult> ShowDialogAsync<T>(string title, string cssClass, ModalParameters? modalParameters = null)
			where T : ComponentBase
		{
			try
			{
				var options = new ModalOptions
				{
					Class = cssClass,
					DisableBackgroundCancel = true,
					UseCustomLayout = true
				};

				IModalReference modal = _modalService.Show<T>(title, modalParameters, options);

				return await modal.Result;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { title, cssClass }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}
	}
}