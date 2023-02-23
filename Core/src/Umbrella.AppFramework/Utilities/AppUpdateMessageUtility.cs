// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Exceptions;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AppFramework.Utilities.Messages;

namespace Umbrella.AppFramework.Utilities;

/// <summary>
/// A utility used to show a message to the user when an application update is available and optionally force
/// the user to upgrade to the new version.
/// </summary>
/// <seealso cref="IAppUpdateMessageUtility"/>
public class AppUpdateMessageUtility : IAppUpdateMessageUtility
{
	private readonly ILogger<AppUpdateMessageUtility> _logger;
	private readonly IDialogUtility _dialogUtility;
	private readonly IUriNavigator _uriNavigator;

	/// <inheritdoc />
	public event Func<bool, string, Task> OnShow
	{
		add => WeakReferenceMessenger.Default.TryRegister<AppUpdateStateChangedMessage>(value.Target, (_, args) => _ = value.Invoke(args.Value.updateRequired, args.Value.message));
		remove => WeakReferenceMessenger.Default.Unregister<AppUpdateStateChangedMessage>(value.Target);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AppUpdateMessageUtility"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="dialogUtility">The dialog utility.</param>
	/// <param name="uriNavigator">The URI navigator.</param>
	public AppUpdateMessageUtility(
		ILogger<AppUpdateMessageUtility> logger,
		IDialogUtility dialogUtility,
		IUriNavigator uriNavigator)
	{
		_logger = logger;
		_dialogUtility = dialogUtility;
		_uriNavigator = uriNavigator;
	}

	/// <inheritdoc />
	public async ValueTask ShowAsync(bool updateRequired, string message, string? storeLink)
	{
		try
		{
			_ = WeakReferenceMessenger.Default.Send(new AppUpdateStateChangedMessage((updateRequired, message)));

			string title = updateRequired ? "Update Required" : "Update Available";

			if (!string.IsNullOrEmpty(storeLink))
			{
				if (updateRequired)
				{
					await _dialogUtility.ShowMessageAsync(message, title, "Update");
					await _uriNavigator.OpenAsync(storeLink!, true);
				}
				else
				{
					bool openLink = await _dialogUtility.ShowConfirmMessageAsync(message, title, "Update");

					if (openLink)
						await _uriNavigator.OpenAsync(storeLink!, true);
				}
			}
			else
			{
				// No store link is available so we can only show the message.
				await _dialogUtility.ShowMessageAsync(message, title, "Close");
			}
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { updateRequired, message }))
		{
			throw new UmbrellaAppFrameworkException("There has been a problem showing the app update message.", exc);
		}
	}
}