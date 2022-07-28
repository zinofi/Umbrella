// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.AspNetCore.WebUtilities.Extensions;

/// <summary>
/// Extension methods that operate on <see cref="IEnumerable{T}" /> instances.
/// </summary>
public static class IEnumerableExtensions
{
	/// <summary>
	/// Converts the specified validation results to a <see cref="ModelStateDictionary"/> suitable for use with responses such as the <see cref="ValidationProblemDetails"/> type.
	/// </summary>
	/// <param name="validationResults">The validation results.</param>
	/// <returns>The <see cref="ModelStateDictionary" />.</returns>
	public static ModelStateDictionary ToModelStateDictionary(this IEnumerable<ValidationResult> validationResults)
	{
		var dictionary = new ModelStateDictionary();

		dictionary.AddValidationResults(validationResults);

		return dictionary;
	}
}