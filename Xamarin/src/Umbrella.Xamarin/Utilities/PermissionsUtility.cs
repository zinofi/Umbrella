// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Xamarin.Exceptions;
using Umbrella.Xamarin.Utilities.Abstractions;
using Umbrella.Xamarin.Utilities.Options;
using Xamarin.Essentials;

namespace Umbrella.Xamarin.Utilities
{
	/// <summary>
	/// A utility that is a wrapper around the <see cref="Permissions"/> functionality
	/// for checking if a requested device permission has been granted using a workflow that assists the end user
	/// in enabling such permissions where they have been previously denied.
	/// </summary>
	/// <seealso cref="IPermissionsUtility"/>
	public class PermissionsUtility : IPermissionsUtility
	{
		private readonly ILogger _logger;
		private readonly IDialogUtility _dialogUtility;
		private readonly PermissionsUtilityOptions _options;

		/// <summary>
		/// Initializes a new instance of the <see cref="PermissionsUtility"/> type.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dialogUtility">The dialog utility.</param>
		/// <param name="options">The options.</param>
		public PermissionsUtility(
			ILogger<PermissionsUtility> logger,
			IDialogUtility dialogUtility,
			PermissionsUtilityOptions options)
		{
			_logger = logger;
			_dialogUtility = dialogUtility;
			_options = options;
		}

		/// <inheritdoc />
		public async Task<bool> CheckAndRequestPermissionAsync<TPermission>()
			where TPermission : Permissions.BasePermission, new()
		{
			try
			{
				ValueTask ShowDeniedErrorMessage()
				{
					string message = typeof(TPermission) switch
					{
						var type when _options.DeniedErrorMessages.ContainsKey(type) => _options.DeniedErrorMessages[type],
						_ => _options.GenericDeniedErrorMessage
					};

					return _dialogUtility.ShowDangerMessageAsync(message, "Permissions Denied");
				};

				var permission = new TPermission();
				var status = await permission.CheckStatusAsync();

				if (status == PermissionStatus.Granted)
					return true;

				if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
				{
					// Prompt the user to turn on in settings
					// On iOS once a permission has been denied it may not be requested again from the application.

					await ShowDeniedErrorMessage();
					return false;
				}

				if (Permissions.ShouldShowRationale<TPermission>())
				{
					// Prompt the user with additional information as to why the permission is needed
					string explanationMessage = typeof(TPermission) switch
					{
						var type when _options.ExplanationMessages.ContainsKey(type) => _options.ExplanationMessages[type],
						_ => _options.GenericExplanationMessage
					};

					await _dialogUtility.ShowInfoMessageAsync(explanationMessage, "Permissions Required");
				}

				if (await Permissions.RequestAsync<TPermission>() == PermissionStatus.Granted)
					return true;

				await ShowDeniedErrorMessage();
				return false;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { PermissionType = typeof(TPermission).FullName }))
			{
				throw new UmbrellaXamarinException("There has been a problem checking the specified permission.");
			}
		}
	}
}