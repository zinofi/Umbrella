using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
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
		private readonly ConcurrentDictionary<Type, IReadOnlyCollection<AuthorizeAttribute>> _authorizationAttributeCache = new ConcurrentDictionary<Type, IReadOnlyCollection<AuthorizeAttribute>>();

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
		private readonly IAppAuthHelper _appAuthHelper;
		private readonly IAuthorizationService _authorizationService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaDialogUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dialogTracker">The dialog tracker.</param>
		/// <param name="modalService">The modal service.</param>
		/// <param name="appAuthHelper">The auth helper.</param>
		/// <param name="authorizationService">The authorization service.</param>
		public UmbrellaDialogUtility(
			ILogger<UmbrellaDialogUtility> logger,
			IDialogTracker dialogTracker,
			IModalService modalService,
			IAppAuthHelper appAuthHelper,
			IAuthorizationService authorizationService)
		{
			_logger = logger;
			_dialogTracker = dialogTracker;
			_modalService = modalService;
			_appAuthHelper = appAuthHelper;
			_authorizationService = authorizationService;
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
		public async ValueTask ShowDangerMessageAsync(string message = "An unknown error has occurred. Please try again.", string title = "Error", string closeButtonText = "Close")
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
		public ValueTask<ModalResult> ShowDialogAsync<T>(string title, string cssClass, ModalParameters? modalParameters = null)
			where T : ComponentBase => ShowDialogAsync<T, ModalResult>(title, cssClass, modalParameters);

		/// <inheritdoc />
		public async ValueTask<TResult> ShowDialogAsync<T, TResult>(string title, string cssClass, ModalParameters? modalParameters = null)
			where T : ComponentBase
			where TResult : ModalResult
		{
			try
			{
				IReadOnlyCollection<AuthorizeAttribute> authorizeAttributes = _authorizationAttributeCache.GetOrAdd(typeof(T), key => key.GetCustomAttributes<AuthorizeAttribute>(true).ToArray());

				if (authorizeAttributes.Count > 0)
				{
					static void ThrowAccessDeniedException() => throw new UnauthorizedAccessException("The current user is not permitted to access the specified dialog.");

					ClaimsPrincipal claimsPrincipal = await _appAuthHelper.GetCurrentClaimsPrincipalAsync();

					if (!claimsPrincipal.Identity.IsAuthenticated)
						ThrowAccessDeniedException();

					// We will now check all authorization attributes. The first one that fails will throw an exception.
					foreach (AuthorizeAttribute authorizeAttribute in authorizeAttributes)
					{
						bool authorized = false;

						if (string.IsNullOrEmpty(authorizeAttribute.Roles) && string.IsNullOrEmpty(authorizeAttribute.Policy))
						{
							// No custom policy or roles have been specified. Just authorize based on whether or not the user
							// is authenticated.
							authorized = claimsPrincipal.Identity.IsAuthenticated;
						}
						else
						{
							// Start by assuming both types of checks are authorized.
							// If roles and/or a policy have been provided they can be invalidated.
							bool rolesAuthorized = true;
							bool policyAuthorized = true;

							if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
							{
								string[] roles = authorizeAttribute.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);

								if (roles.Length > 0)
									rolesAuthorized = roles.Any(x => claimsPrincipal.IsInRole(x));
							}

							if (!string.IsNullOrEmpty(authorizeAttribute.Policy))
							{
								AuthorizationResult authResult = await _authorizationService.AuthorizeAsync(claimsPrincipal, authorizeAttribute.Policy);
								policyAuthorized = authResult.Succeeded;
							}

							authorized = rolesAuthorized && policyAuthorized;
						}

						if (!authorized)
							ThrowAccessDeniedException();
					}
				}

				var options = new ModalOptions
				{
					Class = cssClass,
					DisableBackgroundCancel = true,
					UseCustomLayout = true,
					ContentScrollable = true
				};

				IModalReference modal = _modalService.Show<T>(title, modalParameters, options);

				return (TResult)await modal.Result;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { title, cssClass }, returnValue: true))
			{
				throw new UmbrellaWebComponentException("There has been a problem showing the dialog.", exc);
			}
		}
	}
}