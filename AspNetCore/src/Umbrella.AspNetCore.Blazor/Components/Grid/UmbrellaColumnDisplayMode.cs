namespace Umbrella.AspNetCore.Blazor.Components.Grid;

/// <summary>
/// Specifies how a <see cref="UmbrellaColumn{TItem, TValue}"/> component should be displayed inside an <see cref="UmbrellaGrid{TItem}"/>.
/// </summary>
public enum UmbrellaColumnDisplayMode
{
	/// <summary>
	/// The column will be rendered in full including as a search option if applicable.
	/// </summary>
	Full = 0,

	/// <summary>
	/// The column will not be rendered inside the grid. It will only be used for filtering purposes, if applicable.
	/// </summary>
	SearchOptionsOnly = 1,

	/// <summary>
	/// The column will not be rendered inside the grid and will not be available for filtering purposes.
	/// </summary>
	None = 2
}