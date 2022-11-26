// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Umbrella.Utilities.DataAnnotations.Enumerations;

namespace Umbrella.Utilities.DataAnnotations.Abstractions;

/// <summary>
/// A validation utility used to perform validation on an object instance which uses <see cref="ValidationAttribute"/>s.
/// </summary>
public interface IUmbrellaValidator
{
	/// <summary>
	/// Validates the specified item and returns the results.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="validationType">The type of validation to perform.</param>
	/// <returns>The result of the validation.</returns>
	(bool isValid, IReadOnlyCollection<ValidationResult> results) ValidateItemAsync(object item, ValidationType validationType);
}