// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.StateViews;

/// <summary>
/// A state view component used to display an empty message when there is no data to display.
/// </summary>
public partial class EmptyStateView
{
	/// <summary>
	/// Gets or sets the message.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public string? Message { get; set; }

	/// <inheritdoc />
	protected override void OnParametersSet() => Guard.IsNotNullOrWhiteSpace(Message);
}