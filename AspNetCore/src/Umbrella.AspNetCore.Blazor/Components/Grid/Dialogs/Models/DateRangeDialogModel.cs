using System.ComponentModel.DataAnnotations;
using Umbrella.DataAnnotations;

namespace Umbrella.AspNetCore.Blazor.Components.Grid.Dialogs.Models;

/// <summary>
/// The model used by the <see cref="DateRangeDialog"/>.
/// </summary>
public class DateRangeDialogModel
{
	/// <summary>
	/// Gets or sets the Start Date.
	/// </summary>
	[Required(ErrorMessage = "Please select a start date.")]
	[LessThanOrEqualTo(nameof(EndDate), ErrorMessage = "Please select a date before or the same as the end date.")]
	public DateTime? StartDate { get; set; }

	/// <summary>
	/// Gets or sets the End Date.
	/// </summary>
	[Required(ErrorMessage = "Please select a end date.")]
	[GreaterThanOrEqualTo(nameof(StartDate), ErrorMessage = "Please select a date after or the same as the start date.")]
	public DateTime? EndDate { get; set; }
}