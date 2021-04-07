namespace Umbrella.AspNetCore.Blazor.Components.Grid
{
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
		Search
	}
}