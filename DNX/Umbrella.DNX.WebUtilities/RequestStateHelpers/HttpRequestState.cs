using Umbrella.WebUtilities.RequestStateHelpers.Interfaces;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DNX.WebUtilities.RequestStateHelpers
{
    public class HttpRequestState : IRequestState
    {
        #region Private Members
        private readonly HttpContext m_Context;
        #endregion

        #region Constructors
        public HttpRequestState(HttpContext context = null)
        {
            m_Context = context;
        }
        #endregion

        #region IRequestState Members
        public T Get<T>() where T : class
        {
            return Get<T>(typeof(T).FullName);
        }

        public T Get<T>(string key) where T : class
        {
            return m_Context.Items.Keys.Contains(key)
                ? m_Context.Items[key] as T
                : null;
        }

        public void Store<T>(T value)
        {
            Store(typeof(T).FullName, value);
        }

        public void Store<T>(string key, T value)
        {
            m_Context.Items[key] = value;
        }
        #endregion
    }
}