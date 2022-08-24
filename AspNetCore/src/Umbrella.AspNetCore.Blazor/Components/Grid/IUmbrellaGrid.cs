// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AspNetCore.Blazor.Components.Grid;

/// <summary>
/// An interface that encapsulates the core functionality of the <see cref="UmbrellaGrid{TItem}" /> component.
/// </summary>
public interface IUmbrellaGrid<TItem>
{
	/// <summary>
	/// Gets or sets the render mode. Defaults to <see cref="UmbrellaGridRenderMode.Full"/>.
	/// </summary>
	UmbrellaGridRenderMode RenderMode { get; }

	/// <summary>
	/// Gets the name of the first column property.
	/// </summary>
	string? FirstColumnPropertyName { get; }

	/// <summary>
	/// Adds the column definition to the grid.
	/// </summary>
	/// <param name="column">The column.</param>
	void AddColumnDefinition(UmbrellaColumnDefinition<TItem> column);

	/// <summary>
	/// Sets the column scan as having been completed.
	/// </summary>
	void SetColumnScanCompleted();
}