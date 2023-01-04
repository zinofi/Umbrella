// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AppFramework.Shared.Models;

/// <summary>
/// A no-op result model. This exists for use when specifying generic parameters for types that make use of the AppFramework types
/// but don't support updating items.
/// </summary>
/// <seealso cref="IUpdateResultModel" />
public class NoopUpdateResultModel : IUpdateResultModel
{
	/// <inheritdoc />
	public string ConcurrencyStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}