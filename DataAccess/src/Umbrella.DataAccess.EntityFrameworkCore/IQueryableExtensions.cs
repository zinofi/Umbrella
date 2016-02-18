using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.EntityFrameworkCore
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TEntity> IncludeMap<TEntity>(this IQueryable<TEntity> items, IncludeMap<TEntity> map) where TEntity : class
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
    }
}