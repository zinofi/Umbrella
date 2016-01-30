using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Umbrella.DataAccess.Interfaces;

namespace Umbrella.DataAccess
{
    public class IncludeMap<TEntity> : IIncludeMap<TEntity>
    {
        #region Private Members
        private readonly HashSet<Expression<Func<TEntity, object>>> m_Includes = new HashSet<Expression<Func<TEntity, object>>>();
        #endregion

        #region Constructors
        public IncludeMap(params Expression<Func<TEntity, object>>[] paths)
        {
            foreach (var path in paths)
            {
                m_Includes.Add(path);
            }
        }
        #endregion

        #region IInclude<TEntity> Members
        public HashSet<Expression<Func<TEntity, object>>> Includes => m_Includes;

        public IEnumerator<Expression<Func<TEntity, object>>> GetEnumerator() => m_Includes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_Includes.GetEnumerator();
        #endregion
    }
}