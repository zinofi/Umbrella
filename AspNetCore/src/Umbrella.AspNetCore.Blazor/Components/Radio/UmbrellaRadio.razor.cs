using System;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Umbrella.Utilities;

namespace Umbrella.AspNetCore.Blazor.Components.Radio
{
	public abstract class UmbrellaRadioBase<TValue> : InputBase<TValue>
	{
		public string Id { get; set; } = Guid.NewGuid().ToString("N");

		[Parameter]
		public string GroupName { get; set; } = null!;

		[Parameter]
		public string Text { get; set; } = null!;

		/// <inheritdoc />
		protected override void OnParametersSet()
		{
			base.OnParametersSet();

			Guard.ArgumentNotNullOrWhiteSpace(GroupName, nameof(GroupName));
		}

		[Parameter]
		public TValue SelectedValue { get; set; } = default!;

		protected void OnValueChange(ChangeEventArgs args) => CurrentValueAsString = args.Value.ToString();

		/// <inheritdoc />
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