namespace Umbrella.AspNetCore.Blazor.Components.Grid
{
	/// <summary>
	/// Specifies the type of the items displayed when using the <see cref="UmbrellaColumnFilterType.Options"/> filter type.
	/// </summary>
	public enum UmbrellaColumnFilterOptionsType
	{
		/// <summary>
		/// Strings.
		/// </summary>
		String,

		/// <summary>
		/// <see langword="true"/> or <see langword="false"/>.
		/// </summary>
		Boolean,

		/// <summary>
		/// The values specified using a <see cref="System.Enum" />.
		/// </summary>
		Enum
	}
}