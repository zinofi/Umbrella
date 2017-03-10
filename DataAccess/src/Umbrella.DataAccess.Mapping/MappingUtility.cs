using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbrella.DataAccess.Abstractions.Interfaces;

namespace Umbrella.DataAccess.Mapping
{
    public class MappingUtility : IMappingUtility
    {
        #region Private Members
        private readonly IMapper m_Mapper;
        #endregion

        #region Constructors
        public MappingUtility(IMapper mapper)
        {
            m_Mapper = mapper;
        }
        #endregion

        #region IMappingUtility Members
        public List<TEntity> UpdateItemsList<TModel, TEntity>(IEnumerable<TModel> modelItems, IEnumerable<TEntity> existingItems, Func<TModel, TEntity, bool> matchSelector, Action<TEntity> newEntityAction = null, Func<TEntity, bool> autoInclusionSelector = null, params Action<TModel, TEntity>[] innerActions)
            where TEntity : class, IEntity
        {
            //If there is nothing to process, just return an empty list for the target entity type
            if (modelItems == null)
                return new List<TEntity>();

            var updatedList = new List<TEntity>();

            //Auto include entity items that match the autoIncludeSelector if it isn't null
            if (autoInclusionSelector != null)
                updatedList.AddRange(existingItems.Where(autoInclusionSelector));

            foreach (var item in modelItems)
            {
                TEntity entity = existingItems.SingleOrDefault(x => matchSelector(item, x));

                // No existing item with this id, so add a new one
                if (entity == null)
                {
                    entity = m_Mapper.Map<TEntity>(item);

                    //Make sure the mapped entity has an id of 0 to avoid errors with having mapped an existing item that belongs
                    //to something like a different foreign key relationship
                    entity.Id = 0;

                    newEntityAction?.Invoke(entity);

                    updatedList.Add(entity);
                }
                else // Existing item found, so map to existing instance
                {
                    m_Mapper.Map(item, entity);
                    updatedList.Add(entity);
                }

                if (innerActions != null && innerActions.Length > 0)
                {
                    foreach (var action in innerActions)
                    {
                        action(item, entity);
                    }
                }
            }

            return updatedList;
        } 
        #endregion
    }
}