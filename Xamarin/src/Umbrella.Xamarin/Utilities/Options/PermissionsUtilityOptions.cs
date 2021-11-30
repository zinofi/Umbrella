// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.Xamarin.Utilities.Options
{
	/// <summary>
	/// Options for use with the <see cref="PermissionsUtility"/> class.
	/// </summary>
	/// <seealso cref="ISanitizableUmbrellaOptions"/>
	/// <seealso cref="IValidatableUmbrellaOptions"/>
	public class PermissionsUtilityOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the generic error message shown when a permission-specific error message has not been provided using the <see cref="DeniedErrorMessages"/> dictionary.
		/// </summary>
		public string GenericDeniedErrorMessage { get; set; } = null!;

		/// <summary>
		/// Gets or sets the generic message shown when a permission-specific explanation message has not been provided using the <see cref="ExplanationMessages"/> dictionary.
		/// </summary>
		public string GenericExplanationMessage { get; set; } = null!;

		/// <summary>
		/// Gets a dictionary containing permission-specific error messages when a permission has been denied.
		/// </summary>
		public Dictionary<Type, string> DeniedErrorMessages { get; } = new Dictionary<Type, string>();

		/// <summary>
		/// Gets a dictionary containing permission-specific explanation messages as to why a permission has been requested.
		/// </summary>
		public Dictionary<Type, string> ExplanationMessages { get; } = new Dictionary<Type, string>();

		/// <inheritdoc />
		public void Sanitize()
		{
			GenericDeniedErrorMessage = GenericDeniedErrorMessage?.Trim()!;
			GenericExplanationMessage = GenericExplanationMessage?.Trim()!;
		}

		/// <inheritdoc />
		public void Validate()
		{
			Guard.ArgumentNotNullOrEmpty(GenericDeniedErrorMessage, nameof(GenericDeniedErrorMessage));
			Guard.ArgumentNotNullOrEmpty(GenericExplanationMessage, nameof(GenericExplanationMessage));
		}
	}
}