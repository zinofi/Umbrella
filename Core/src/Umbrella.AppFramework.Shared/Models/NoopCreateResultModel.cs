// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;

namespace Umbrella.AppFramework.Shared.Models;

public class NoopCreateResultModel<TKey> : ICreateResultModel<TKey>
	where TKey : IEquatable<TKey>
{
	/// <inheritdoc />
	public TKey Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}