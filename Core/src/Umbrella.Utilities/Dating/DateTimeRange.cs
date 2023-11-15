using System.Runtime.InteropServices;

namespace Umbrella.Utilities.Dating;

/// <summary>
/// Represents a date time range.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public record struct DateTimeRange
{
	/// <summary>
	/// Gets or sets the Start Date.
	/// </summary>
	public DateTime StartDate { get; set; }

	/// <summary>
	/// Gets or sets the End Date.
	/// </summary>
	public DateTime EndDate { get; set; }
}