using System.Data.Entity;
using System.Linq;
using Umbrella.DataAccess.EF.Abstractions;

namespace Umbrella.DataAccess.EF6
{
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
		public static IQueryable<TEntity> IncludeMap<TEntity>(this IQueryable<TEntity> items, IncludeMap<TEntity> map)
			where TEntity : class
		{
			if (map == null)
				return items;

			var query = items;

			foreach (var item in map.Includes)
			{
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
}