using System.Globalization;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Umbrella.AspNetCore.Blazor.Components.Input;

/// <summary>
/// An input component for editing numerical values that have a formatted display.
/// </summary>
/// <typeparam name="T">The type of the numerical value.</typeparam>
public class UmbrellaFormattedInputNumber<T> : InputNumber<T>
{
	/// <summary>
	/// The number format to use when displaying the value.
	/// </summary>
	/// <remarks>Defaults to <c>N2</c>.</remarks>
	[Parameter]
	public string Format { get; set; } = "N2";

	/// <inheritdoc />
	protected override void OnParametersSet() => AdditionalAttributes = UmbrellaInputHelper.ApplyAttributes(AdditionalAttributes, ValueExpression);

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		Guard.IsNotNull(builder);

		builder.OpenElement(0, "input");
		builder.AddMultipleAttributes(1, AdditionalAttributes);
		builder.AddAttribute(1, "type", "text");
		builder.AddAttribute(2, "class", CssClass);
		builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValueAsString));
		builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder<string>(this, value => CurrentValueAsString = value, CurrentValueAsString ?? string.Empty));
		builder.CloseElement();
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(T? value)
	{
		if (value is null)
			return string.Empty;

		// Format the value using the specified format
		return string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", value)
			.Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "", StringComparison.CurrentCulture);
	}
}