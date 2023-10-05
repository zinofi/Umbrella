﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Options.Abstractions;
using Umbrella.Xamarin.Utilities.Enumerations;

namespace Umbrella.Xamarin.Utilities.Options;

/// <summary>
/// Options for use with the <see cref="PermissionsUtility"/> class.
/// </summary>
/// <seealso cref="ISanitizableUmbrellaOptions"/>
/// <seealso cref="IValidatableUmbrellaOptions"/>
public class PermissionsUtilityOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the generic error message shown when a permission-specific error message has not been provided using the <see cref="AndroidDeniedErrorMessages"/> or <see cref="IOSDeniedErrorMessages"/> dictionaries.
	/// </summary>
	public string GenericDeniedErrorMessage { get; set; } = null!;

	/// <summary>
	/// Gets or sets the generic message shown when a permission-specific explanation message has not been provided using the <see cref="ExplanationMessages"/> dictionary.
	/// </summary>
	public string GenericExplanationMessage { get; set; } = null!;

	/// <summary>
	/// Gets a dictionary containing Android permission-specific error messages when a permission has been denied.
	/// </summary>
	public Dictionary<PermissionType, string> AndroidDeniedErrorMessages { get; } = new Dictionary<PermissionType, string>();

	/// <summary>
	/// Gets a dictionary containing iOS permission-specific error messages when a permission has been denied.
	/// </summary>
	public Dictionary<PermissionType, string> IOSDeniedErrorMessages { get; } = new Dictionary<PermissionType, string>();

	/// <summary>
	/// Gets a dictionary containing permission-specific explanation messages as to why a permission has been requested.
	/// </summary>
	public Dictionary<PermissionType, string> ExplanationMessages { get; } = new Dictionary<PermissionType, string>();

	/// <inheritdoc />
	public void Sanitize()
	{
		GenericDeniedErrorMessage = GenericDeniedErrorMessage?.Trim()!;
		GenericExplanationMessage = GenericExplanationMessage?.Trim()!;
	}

	/// <inheritdoc />
	public void Validate()
	{
		Guard.IsNotNullOrEmpty(GenericDeniedErrorMessage);
		Guard.IsNotNullOrEmpty(GenericExplanationMessage);
	}
}