﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AppFramework.Shared.Models;

/// <summary>
/// A result model of the operation to create an item.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface ICreateResultModel<TKey>
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// Gets or sets the identifier.
	/// </summary>
	TKey Id { get; set; }
}