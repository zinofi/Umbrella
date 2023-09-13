using System.Runtime.CompilerServices;

namespace Umbrella.AspNetCore.Blazor.Components.Grid;

/// <summary>
/// The type of the filter applied to a column.
/// </summary>
public enum UmbrellaColumnFilterType
{
	/// <summary>
	/// Free text. Renders a text input of type "text".
	/// </summary>
	Text,

	/// <summary>
	/// Email. Renders a text input of type "email".
	/// </summary>
	Email,

	/// <summary>
	/// Numerical. Renders a text input of type "number".
	/// </summary>
	Number,

	/// <summary>
	/// Options displayed using a dropdown.
	/// </summary>
	Options,

	/// <summary>
	/// Free text. Renders a text input of type "search".
	/// </summary>
	Search,

	/// <summary>
	/// Date. Randers a text input of type "date".
	/// </summary>
	Date,

	/// <summary>
	/// Renders a text input of type "text" with an add-on button which when clicked will invoke a specified delegate.
	/// </summary>
	TextAddOnButton,

	/// <summary>
	/// Renders a control that allows a user to enter a start and end date.
	/// Used in conjunction with the <see cref="UmbrellaColumnDateRangeRequiredMode"/> enum.
	/// </summary>
	DateRange,

	/// <summary>
	/// Renders a text input of type "text" that shows a list of autocomplete options when the user finishes typing.
	/// </summary>
	AutoComplete
}

internal static class UmbrellaColumnFilterTypeExtensions
{
	public static string ToControlTypeString(this UmbrellaColumnFilterType value) => value switch
	{
		UmbrellaColumnFilterType.Text => "text",
		UmbrellaColumnFilterType.Email => "email",
		UmbrellaColumnFilterType.Number => "number",
		UmbrellaColumnFilterType.Options => "text", // This will never be used but here as a default.
		UmbrellaColumnFilterType.Search => "search",
		UmbrellaColumnFilterType.Date => "date",
		UmbrellaColumnFilterType.TextAddOnButton => "text",
		UmbrellaColumnFilterType.DateRange => "text", // This will never be used but here as a default.
		UmbrellaColumnFilterType.AutoComplete => "text", // This will never be used but here as a default.
		_ => throw new SwitchExpressionException(value)
	};
}

/// <summary>
/// The mode applied to a <see cref="UmbrellaColumnFilterType.DateRange"/> filter to specify which parts
/// of the date range or required when entering values.
/// </summary>
public enum UmbrellaColumnDateRangeRequiredMode
{
	/// <summary>
	/// Specifies that only the start date is required.
	/// </summary>
	StartDate,

	/// <summary>
	/// Specifies that only the end date is required.
	/// </summary>
	EndDate,

	/// <summary>
	/// Specifies that both the start date and end dates are required.
	/// </summary>
	Both
}