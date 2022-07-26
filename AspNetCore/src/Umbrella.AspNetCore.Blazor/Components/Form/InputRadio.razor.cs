using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Umbrella.AspNetCore.Blazor.Components.Form
{
	/// <summary>
	/// Serves as the base class of the <see cref="InputRadio{TValue}"/> component.
	/// </summary>
	public class InputRadioBase<TValue> : InputBase<TValue>
	{
		/// <summary>
		/// Gets or sets the selected value.
		/// </summary>
		[Parameter]
		public TValue SelectedValue { get; set; } = default!;

		/// <summary>
		/// Called by the Blazor infrastructure when the <see cref="SelectedValue"/> changes.
		/// </summary>
		/// <param name="args">The <see cref="ChangeEventArgs"/>.</param>
		protected void OnValueChange(ChangeEventArgs args) => CurrentValueAsString = args.Value?.ToString();

		/// <inheritdoc />
		protected override bool TryParseValueFromString(string? value, out TValue result, out string errorMessage)
		{
			bool success = BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out var parsedValue);

			if (success && parsedValue is not null)
			{
				result = parsedValue;
				errorMessage = null!;

				return true;
			}
			else
			{
				result = default!;
				errorMessage = $"{FieldIdentifier.FieldName} field isn't valid.";

				return false;
			}
		}
	}
}