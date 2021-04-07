using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Umbrella.Utilities;

namespace Umbrella.AspNetCore.Blazor.Components.StateViews
{
    public abstract class EmptyStateViewBase : ComponentBase
    {
		[Parameter]
		public string? Message { get; set; }

		/// <inheritdoc />
		protected override void OnParametersSet()
		{
			Guard.ArgumentNotNullOrWhiteSpace(Message, nameof(Message));
		}
	}
}