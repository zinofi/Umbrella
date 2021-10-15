using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Exceptions;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Utilities.WeakEventManager.Abstractions;

namespace Umbrella.AppFramework.Utilities
{
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
		private readonly IWeakEventManager _weakEventManager;

		/// <inheritdoc />
		public event Func<bool, string, Task> OnShow
		{
			add => _weakEventManager.AddEventHandler(value);
			remove => _weakEventManager.RemoveEventHandler(value);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppUpdateMessageUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dialogUtility">The dialog utility.</param>
		/// <param name="uriNavigator">The URI navigator.</param>
		/// <param name="weakEventManagerFactory">The weak event manager factory.</param>
		public AppUpdateMessageUtility(
			ILogger<AppUpdateMessageUtility> logger,
			IDialogUtility dialogUtility,
			IUriNavigator uriNavigator,
			IWeakEventManagerFactory weakEventManagerFactory)
		{
			_logger = logger;
			_dialogUtility = dialogUtility;
			_uriNavigator = uriNavigator;
			_weakEventManager = weakEventManagerFactory.Create();
		}

		/// <inheritdoc />
		public async ValueTask ShowAsync(bool updateRequired, string message, string? storeLink)
		{
			try
			{
				IReadOnlyCollection<Task> lstTask = _weakEventManager.RaiseEvent<Task>(nameof(OnShow), updateRequired, message);

				if (lstTask?.Count > 0)
					await Task.WhenAll(lstTask);

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
			catch (Exception exc) when (_logger.WriteError(exc, new { updateRequired, message }, returnValue: true))
			{
				throw new UmbrellaAppFrameworkException("There has been a problem showing the app update message.", exc);
			}
		}
	}
}