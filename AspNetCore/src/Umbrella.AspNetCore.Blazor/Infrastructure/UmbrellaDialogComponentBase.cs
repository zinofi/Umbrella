﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Umbrella.AspNetCore.Blazor.Infrastructure;

/// <summary>
/// A base component to be used with custom dialogs.
/// </summary>
/// <seealso cref="UmbrellaClientComponentBase" />
public abstract class UmbrellaDialogComponentBase : UmbrellaClientComponentBase
{
	/// <summary>
	/// Gets or sets the dialog instance.
	/// </summary>
	[CascadingParameter]
	protected BlazoredModalInstance ModalInstance { get; set; } = null!;

	/// <summary>
	/// Handles the close button click event for the dialog.
	/// </summary>
	/// <returns>An awaitable Task that completed when this operation has completed.</returns>
	protected async Task CloseClickAsync()
	{
		try
		{
			await ModalInstance.CancelAsync();
		}
		catch (Exception exc) when (Logger.WriteError(exc))
		{
			await DialogUtility.ShowDangerMessageAsync();
		}
	}
}