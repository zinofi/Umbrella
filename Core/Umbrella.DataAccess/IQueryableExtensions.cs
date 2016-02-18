using System;
using System.Linq;
using System.Linq.Expressions;
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
    }
}