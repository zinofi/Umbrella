// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.DataAccess.Abstractions;

public record IdEntityPairResult<TEntity, TEntityKey>
	where TEntity : class, IEntity<TEntityKey>
	where TEntityKey : IEquatable<TEntityKey>
{
	public required TEntityKey Id { get; init; }
	public TEntity? Entity { get; init; }
}