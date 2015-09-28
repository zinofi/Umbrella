using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Data.Entity;
using System.Linq.Expressions;
using Umbrella.DataAccess.Interfaces;

namespace Umbrella.DataAccess
{
    public abstract class ReadOnlyRepository<TEntity, TDbContext> : IReadOnlyRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext, new()
    {
        #region Private Members
        private IDataContextFactory<TDbContext> m_DataContextFactory;
        #endregion

        /// <summary>
        /// This Context is shared between all repositories for the specified DbContext across a single HttpRequest
        /// This is to avoid problems when requesting something with one context and saving it with another which may be the case
        /// when dealing with child entities
        /// </summary>
        protected TDbContext Context
        {
            get
            {
                TDbContext contextInstance = m_DataContextFactory.ContextInstance;

                if (contextInstance == null)
                {
                    contextInstance = new TDbContext();
                    m_DataContextFactory.ContextInstance = contextInstance;
                }

                return contextInstance;
            }
        }
        #region Constructors
        public ReadOnlyRepository()
        {
        }

        public ReadOnlyRepository(IDataContextFactory<TDbContext> dataContextFactory)
        {
            m_DataContextFactory = dataContextFactory;
        }
        #endregion

        protected IQueryable<TEntity> Items
        {
            get { return Context.Set<TEntity>(); }
        }

        public List<TEntity> FindAll()
        {
            return Items.ToList();
        }
    }
}