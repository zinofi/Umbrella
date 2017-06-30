using System;
using System.Collections.Generic;
using AutoMapper;
using Umbrella.DataAccess.Abstractions.Interfaces;

namespace Umbrella.DataAccess.Mapping
{
    public interface IMappingUtility
    {
        List<TEntity> UpdateItemsList<TModel, TEntity>(IEnumerable<TModel> modelItems, IEnumerable<TEntity> existingItems, Func<TModel, TEntity, bool> matchSelector, Action<TEntity> newEntityAction = null, Func<TEntity, bool> autoInclusionSelector = null, params Action<TModel, TEntity>[] innerActions) where TEntity : class, IEntity;
        List<TEntity> UpdateItemsList<TModel, TEntity, TEntityKey>(IEnumerable<TModel> modelItems, IEnumerable<TEntity> existingItems, Func<TModel, TEntity, bool> matchSelector, Action<TEntity> newEntityAction = null, Func<TEntity, bool> autoInclusionSelector = null, params Action<TModel, TEntity>[] innerActions) where TEntity : class, IEntity<TEntityKey>;
    }
}