﻿namespace Umbrella.AspNetCore.Blazor.Components.Grid.Options;

/// <summary>
/// Global options used to configure all instances of the Umbrella Grid.
/// </summary>
public class UmbrellaGridOptions
{
	/// <summary>
	/// Specifies whether support for Search Option state is enabled.
	/// </summary>
	public bool IsSearchOptionStateEnabled { get; set; }

	/// <summary>
	/// Specifies whether the Bootstrap CSS Grid should be used.
	/// </summary>
	public bool UseBootstrapCssGrid { get; set; }
}