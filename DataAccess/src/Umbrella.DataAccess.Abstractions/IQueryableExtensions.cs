﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Umbrella.Utilities.Enumerations;

namespace Umbrella.DataAccess.Abstractions
{
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<TSource> OrderBySortDirection<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, SortDirection direction)
        {
            switch (direction)
            {
                default:
                case SortDirection.Ascending:
                    return source.OrderBy(keySelector);
                case SortDirection.Descending:
                    return source.OrderByDescending(keySelector);
            }
        }
    }
}