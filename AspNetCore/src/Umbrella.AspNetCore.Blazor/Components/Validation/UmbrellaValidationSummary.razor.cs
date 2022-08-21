using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.Validation
{
	/// <summary>
	/// A component used to display a collection of <see cref="ValidationResult"/> instances.
	/// </summary>
	/// <seealso cref="ComponentBase" />
	public partial class UmbrellaValidationSummary
	{
		/// <summary>
		/// Gets or sets the heading.
		/// </summary>
		[Parameter]
		public string? Heading { get; set; }

		/// <summary>
		/// Gets or sets the validation results.
		/// </summary>
		[Parameter]
		public IReadOnlyCollection<ValidationResult>? ValidationResults { get; set; }
	}
}