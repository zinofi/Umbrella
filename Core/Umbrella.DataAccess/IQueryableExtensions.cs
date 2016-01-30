using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Umbrella.DataAccess.Interfaces;
using Umbrella.Utilities.Enumerations;

namespace Umbrella.DataAccess
{
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<TEntity> ApplySortOrder<TEntity, TProperty>(this IQueryable<TEntity> unsorted, Expression<Func<TEntity, TProperty>> expression, SortDirection direction) where TEntity : class
        {
            switch (direction)
            {
                default:
                case SortDirection.Ascending:
                    return unsorted.OrderBy(expression);
                case SortDirection.Descending:
                    return unsorted.OrderByDescending(expression);
            }
        }

        public static IQueryable<TEntity> IncludeMap<TEntity>(this IQueryable<TEntity> items, IIncludeMap<TEntity> map) where TEntity : class
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