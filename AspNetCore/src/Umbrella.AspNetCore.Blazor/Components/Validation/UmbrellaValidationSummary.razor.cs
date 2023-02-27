using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Umbrella.AppFramework.Services.Constants;

namespace Umbrella.AspNetCore.Blazor.Components.Validation;

/// <summary>
/// A component used to display a collection of <see cref="ValidationResult"/> instances.
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class UmbrellaValidationSummary
{
	/// <summary>
	/// Gets or sets the heading.
	/// </summary>
	/// <remarks>Defaults to <see cref="ValidationDefaults.ValidationSummaryIntroMessage"/></remarks>
	[Parameter]
	public string Heading { get; set; } = ValidationDefaults.ValidationSummaryIntroMessage;

	/// <summary>
	/// Gets or sets the validation results.
	/// </summary>
	[Parameter]
	public IReadOnlyCollection<ValidationResult>? ValidationResults { get; set; }
}