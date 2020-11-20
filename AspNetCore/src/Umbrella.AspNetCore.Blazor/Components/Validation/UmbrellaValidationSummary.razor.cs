using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.Validation
{
	public partial class UmbrellaValidationSummary
	{
		[Parameter]
		public IReadOnlyCollection<ValidationResult>? ValidationResults { get; set; }
	}
}