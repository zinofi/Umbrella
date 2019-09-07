using System;
using System.Collections.Generic;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.DataAccess.Mapping
{
	/// <summary>
	/// A utility class used to update a collection of existing items using an incoming items collection.
	/// </summary>
	public interface IMappingUtility
	{
		IReadOnlyCollection<TEntity> UpdateItemsList<TModel, TEntity>(IEnumerable<TModel> modelItems, IEnumerable<TEntity> existingItems, Func<TModel, TEntity, bool> matchSelector, Action<TEntity> newEntityAction = null, Func<TEntity, bool> autoInclusionSelector = null, params Action<TModel, TEntity>[] innerActions) where TEntity : class, IEntity;
		IReadOnlyCollection<TEntity> UpdateItemsList<TModel, TEntity, TEntityKey>(IEnumerable<TModel> modelItems, IEnumerable<TEntity> existingItems, Func<TModel, TEntity, bool> matchSelector, Action<TEntity> newEntityAction = null, Func<TEntity, bool> autoInclusionSelector = null, params Action<TModel, TEntity>[] innerActions) where TEntity : class, IEntity<TEntityKey>;
	}
}