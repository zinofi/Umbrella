using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.StateViews
{
	/// <summary>
	/// A state view component used to display a loading message. Defaults to "Loading... Please wait.".
	/// </summary>
	public partial class LoadingStateView
	{
		/// <summary>
		/// Gets or sets the message. Defaults to "Loading... Please wait.".
		/// </summary>
		[Parameter]
		public string Message { get; set; } = "Loading... Please wait.";
	}
}