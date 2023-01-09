﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Xamarin.Exceptions;
using Umbrella.Xamarin.Utilities.Abstractions;
using Umbrella.Xamarin.Utilities.Enumerations;
using Umbrella.Xamarin.Utilities.Options;
using Xamarin.Essentials;

namespace Umbrella.Xamarin.Utilities;

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

	private readonly Dictionary<PermissionType, IReadOnlyCollection<Permissions.BasePermission>> _androidMappings = new()
	{
		[PermissionType.NewPhoto] = new Permissions.BasePermission[] { new Permissions.Camera(), new Permissions.StorageWrite() },
		[PermissionType.NewVideo] = new Permissions.BasePermission[] { new Permissions.Camera(), new Permissions.StorageWrite() },
		[PermissionType.SavedPhoto] = new Permissions.BasePermission[] { new Permissions.StorageRead() },
		[PermissionType.SavedVideo] = new Permissions.BasePermission[] { new Permissions.StorageRead() },
		[PermissionType.File] = new Permissions.BasePermission[] { new Permissions.StorageRead() },
	};

	private readonly Dictionary<PermissionType, IReadOnlyCollection<Permissions.BasePermission>> _iOSMappings = new()
	{
		[PermissionType.NewPhoto] = new Permissions.BasePermission[] { new Permissions.Camera() },
		[PermissionType.NewVideo] = new Permissions.BasePermission[] { new Permissions.Camera(), new Permissions.Microphone() },
		[PermissionType.SavedPhoto] = new Permissions.BasePermission[] { new Permissions.Photos() },
		[PermissionType.SavedVideo] = new Permissions.BasePermission[] { new Permissions.Photos() },
		[PermissionType.File] = Array.Empty<Permissions.BasePermission>()
	};

	private readonly HashSet<Type> _previousFailuresMappings = new();

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
	public async Task<bool> CheckAndRequestPermissionAsync(PermissionType permissionType)
	{
		try
		{
			IReadOnlyCollection<Permissions.BasePermission> lstRequiredPermission = DeviceInfo.Platform switch
			{
				var platform when platform == DevicePlatform.iOS => _iOSMappings[permissionType],
				var platform when platform == DevicePlatform.Android => _androidMappings[permissionType],
				_ => throw new NotSupportedException("The current platform is not supported.")
			};

			bool success = true;
			bool showRationale = false;

			var lstDeniedPermission = new List<Permissions.BasePermission>();

			foreach (var permission in lstRequiredPermission)
			{
				var status = await permission.CheckStatusAsync();

				if (status == PermissionStatus.Granted)
					continue;

				if (((DeviceInfo.Platform == DevicePlatform.Android && status == PermissionStatus.Denied) || (DeviceInfo.Platform == DevicePlatform.iOS)) && !_previousFailuresMappings.Contains(permission.GetType()))
				{
					showRationale = true;
					lstDeniedPermission.Add(permission);
				}

				success = false;
			}

			if (success)
				return true;

			if (showRationale)
			{
				// Prompt the user with additional information as to why the permission is needed
				string explanationMessage = permissionType switch
				{
					var _ when _options.ExplanationMessages.ContainsKey(permissionType) => _options.ExplanationMessages[permissionType],
					_ => _options.GenericExplanationMessage
				};

				await _dialogUtility.ShowInfoMessageAsync(explanationMessage, "Permission Required");

				foreach (var permission in lstDeniedPermission)
				{
					// We need all permissions to be granted. Fail on the first one.
					if (await permission.RequestAsync() != PermissionStatus.Granted)
					{
						await _dialogUtility.ShowDangerMessageAsync("You have not granted the required permissions. Please try again.");

						// Track this so that the next time we get a failure for this permission we don't try again.
						_ = _previousFailuresMappings.Add(permission.GetType());

						return false;
					}
					else
					{
						success = true;
					}
				}
			}
			else
			{
				// We can't re-request the permission, e.g. the platform is iOS. We can only show an error message and direct
				// the user to the native iOS Settings app.
				string message = permissionType switch
				{
					var _ when DeviceInfo.Platform == DevicePlatform.Android && _options.AndroidDeniedErrorMessages.ContainsKey(permissionType) => _options.AndroidDeniedErrorMessages[permissionType],
					var _ when DeviceInfo.Platform == DevicePlatform.iOS && _options.IOSDeniedErrorMessages.ContainsKey(permissionType) => _options.IOSDeniedErrorMessages[permissionType],
					_ => _options.GenericDeniedErrorMessage
				};

				await _dialogUtility.ShowDangerMessageAsync(message, "Permission Denied");
			}

			return success;
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { permissionType }))
		{
			throw new UmbrellaXamarinException("There has been a problem checking the specified permission.");
		}
	}
}