using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Umbrella.AspNetCore.Blazor.Components.Checkbox
{
	/// <summary>
	/// Serves as the base class of the <see cref="UmbrellaCheckbox"/> component.
	/// </summary>
	public abstract class UmbrellaCheckboxBase : InputBase<bool>
	{
		/// <summary>
		/// Gets or sets the ID of the checkbox. This is used to associate the checkbox input with it's label.
		/// </summary>
		public string Id { get; set; } = Guid.NewGuid().ToString("N");

		/// <summary>
		/// Gets or sets the checkbox label text. If <see cref="ChildContent"/> has been specified, that will take precedence.
		/// </summary>
		[Parameter]
		public string Text { get; set; } = null!;

		/// <summary>
		/// Gets or sets the custom content of the label. If this is null, the <see cref="Text"/> is used as the label content.
		/// </summary>
		[Parameter]
		public RenderFragment? ChildContent { get; set; }

		/// <inheritdoc />
		protected override bool TryParseValueFromString(string? value, out bool result, out string validationErrorMessage)
			=> throw new NotImplementedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
	}
}