using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Umbrella.AspNetCore.Blazor.Components.Checkbox
{
	public abstract class UmbrellaCheckboxBase : InputBase<bool>
	{
		public string Id { get; set; } = Guid.NewGuid().ToString("N");

		[Parameter]
		public string Text { get; set; } = null!;

		[Parameter]
		public RenderFragment? ChildContent { get; set; }

		protected override bool TryParseValueFromString(string value, out bool result, out string validationErrorMessage)
			=> throw new NotImplementedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
	}
}