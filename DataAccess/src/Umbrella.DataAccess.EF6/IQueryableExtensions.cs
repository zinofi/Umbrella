using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.EF6
{
    public static class IQueryableExtensions
    {
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

        public static IQueryable<TEntity> TrackChanges<TEntity>(this IQueryable<TEntity> items, bool trackChanges) where TEntity : class
            => trackChanges ? items : items.AsNoTracking();
    }
}