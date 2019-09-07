using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;

namespace Umbrella.DataAccess.Mapping
{
	/// <summary>
	/// A utility class used to update a collection of existing items using an incoming items collection. This implementation leverages AutoMapper internally using
	/// a constructor injected instance of <see cref="IMapper"/>.
	/// </summary>
	/// <seealso cref="IMappingUtility" />
	public class MappingUtility : IMappingUtility
	{
		#region Private Members
		private readonly ILogger _log;
		private readonly IMapper _mapper;
		#endregion

		#region Constructors
		public MappingUtility(
			ILogger<MappingUtility> logger,
			IMapper mapper)
		{
			_log = logger;
			_mapper = mapper;
		}
		#endregion

		#region IMappingUtility Members
		public IReadOnlyCollection<TEntity> UpdateItemsList<TModel, TEntity>(IEnumerable<TModel> modelItems, IEnumerable<TEntity> existingItems, Func<TModel, TEntity, bool> matchSelector, Action<TEntity> newEntityAction = null, Func<TEntity, bool> autoInclusionSelector = null, params Action<TModel, TEntity>[] innerActions)
			where TEntity : class, IEntity
			=> UpdateItemsList<TModel, TEntity, int>(modelItems, existingItems, matchSelector, newEntityAction, autoInclusionSelector, innerActions);

		public IReadOnlyCollection<TEntity> UpdateItemsList<TModel, TEntity, TEntityKey>(IEnumerable<TModel> modelItems, IEnumerable<TEntity> existingItems, Func<TModel, TEntity, bool> matchSelector, Action<TEntity> newEntityAction = null, Func<TEntity, bool> autoInclusionSelector = null, params Action<TModel, TEntity>[] innerActions)
			where TEntity : class, IEntity<TEntityKey>
		{
			try
			{
				// If there is nothing to process, just return an empty list for the target entity type
				if (modelItems == null)
					return Array.Empty<TEntity>();

				var updatedList = new List<TEntity>();

				// Auto include entity items that match the autoIncludeSelector if it isn't null
				if (autoInclusionSelector != null)
					updatedList.AddRange(existingItems.Where(autoInclusionSelector));

				foreach (var item in modelItems)
				{
					TEntity entity = existingItems.SingleOrDefault(x => matchSelector(item, x));

					// No existing item with this id, so add a new one
					if (entity == null)
					{
						entity = _mapper.Map<TEntity>(item);

						// Make sure the mapped entity has an default id value to avoid errors with having mapped an existing item that belongs
						// to something like a different foreign key relationship
						entity.Id = default;

						newEntityAction?.Invoke(entity);

						updatedList.Add(entity);
					}
					else // Existing item found, so map to existing instance
					{
						_mapper.Map(item, entity);
						updatedList.Add(entity);
					}

					if (innerActions?.Length > 0)
					{
						foreach (var action in innerActions)
						{
							action(item, entity);
						}
					}
				}

				return updatedList;
			}
			catch (Exception exc) when (_log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaDataAccessException("There has been a problem updating the target items list.", exc);
			}
		}
		#endregion
	}
}