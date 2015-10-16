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
            //TODO: Check this is ok
            return m_Context.Features.Get<T>();
        }

        public T Get<T>(string key) where T : class
        {
            return m_Context.Items[key] as T;
        }

        public void Store<T>(T something)
        {
            throw new NotImplementedException();
        }

        public void Store<T>(string key, T something)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}