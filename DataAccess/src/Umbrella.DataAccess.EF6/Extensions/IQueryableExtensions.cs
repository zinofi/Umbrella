// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Data.Entity;
using System.Linq;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.DataAccess.EF6.Extensions;

/// <summary>
/// Extensions for the <see cref="IQueryable{T}"/> interface.
/// </summary>
public static class IQueryableExtensions
{
	/// <summary>
	/// Add the specified include map to the query.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="items">The items.</param>
	/// <param name="map">The include map.</param>
	/// <returns>The query.</returns>
	public static IQueryable<TEntity> IncludeMap<TEntity>(this IQueryable<TEntity> items, IncludeMap<TEntity>? map)
		where TEntity : class
	{
		if (map is null)
			return items;

		var query = items;

		foreach (var item in map.Includes)
		{
			// TODO: Replicate the switch to use the string path as per EF Core?
			query = query.Include(item);
		}

		return query;
	}

	/// <summary>
	/// Specifies whether or not to create tracking entries for the current query.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="items">The items.</param>
	/// <param name="trackChanges">if set to <see langwrod="true"/> tracks query changes.</param>
	/// <returns>The query.</returns>
	public static IQueryable<TEntity> TrackChanges<TEntity>(this IQueryable<TEntity> items, bool trackChanges) where TEntity : class
		=> trackChanges ? items : items.AsNoTracking();
}