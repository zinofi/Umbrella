using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Umbrella.AspNetCore.Blazor.Components.Grid.Dialogs.Models;

public class DateRangeDialogModel
{
	[Required(ErrorMessage = "Please select a start date.")]
	public DateTime? StartDate { get; set; }

	[Required(ErrorMessage = "Please select a end date.")]
	public DateTime? EndDate { get; set; }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(DateRange))]
internal partial class DateRangeJsonSerializerContext : JsonSerializerContext
{
}

[StructLayout(LayoutKind.Auto)]
public struct DateRange
{
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
}