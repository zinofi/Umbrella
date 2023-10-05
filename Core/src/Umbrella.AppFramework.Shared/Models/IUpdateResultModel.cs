// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.Utilities.Data.Concurrency;

namespace Umbrella.AppFramework.Shared.Models;

/// <summary>
/// A result model of the operation to update an item.
/// </summary>
/// <seealso cref="IConcurrencyStamp" />
public interface IUpdateResultModel : IConcurrencyStamp
{
	// TODO: Consider adding an Id property for convenience.
}