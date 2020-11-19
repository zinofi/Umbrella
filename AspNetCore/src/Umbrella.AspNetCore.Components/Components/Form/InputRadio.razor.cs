using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Umbrella.AspNetCore.Components.Components.Form
{
	public class InputRadioBase<TValue> : InputBase<TValue>
	{
		[Parameter]
		public TValue SelectedValue { get; set; } = default!;

		protected void OnValueChange(ChangeEventArgs args) => CurrentValueAsString = args.Value.ToString();

		protected override bool TryParseValueFromString(string value, out TValue result, out string errorMessage)
		{
			bool success = BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out var parsedValue);

			if (success)
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