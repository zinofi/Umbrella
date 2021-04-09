using Microsoft.AspNetCore.Components;
using Umbrella.Utilities;

namespace Umbrella.AspNetCore.Blazor.Components.StateViews
{
	/// <summary>
	/// A state view component used to display an empty message when there is no data to display.
	/// </summary>
	public partial class EmptyStateView
    {
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		[Parameter]
		public string? Message { get; set; }

		/// <inheritdoc />
		protected override void OnParametersSet() => Guard.ArgumentNotNullOrWhiteSpace(Message, nameof(Message));
	}
}