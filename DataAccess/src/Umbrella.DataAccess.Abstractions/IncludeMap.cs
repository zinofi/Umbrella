using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions.Interfaces;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.Abstractions
{
    public class IncludeMap<TEntity> : IEnumerable<Expression<Func<TEntity, object>>>
    {
        #region Private Members
        private readonly HashSet<Expression<Func<TEntity, object>>> m_Includes = new HashSet<Expression<Func<TEntity, object>>>();
        private readonly HashSet<string> m_PropertyNames = new HashSet<string>();
        #endregion

        #region Public Properties
        public HashSet<Expression<Func<TEntity, object>>> Includes => m_Includes;
        public HashSet<string> PropertyNames => m_PropertyNames;
        #endregion

        #region Constructors
        public IncludeMap(params Expression<Func<TEntity, object>>[] paths)
        {
            foreach (var path in paths)
            {
                m_Includes.Add(path);

                string propertyName = path.GetMemberName(false);

                if(!string.IsNullOrEmpty(propertyName))
                    m_PropertyNames.Add(propertyName);
            }
        }
        #endregion

        #region IEnumerable Members
        public IEnumerator<Expression<Func<TEntity, object>>> GetEnumerator() => m_Includes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_Includes.GetEnumerator();
        #endregion
    }
}