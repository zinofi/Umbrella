// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AppFramework.Shared.Models;

/// <summary>
/// A no-op result model. This exists for use when specifying generic parameters for types that make use of the AppFramework types
/// but don't support creating items.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public class NoopCreateResultModel<TKey> : ICreateResultModel<TKey>
	where TKey : IEquatable<TKey>
{
	/// <inheritdoc />
	public TKey Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}