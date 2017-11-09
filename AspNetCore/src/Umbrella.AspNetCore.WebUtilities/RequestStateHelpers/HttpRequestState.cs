using Umbrella.WebUtilities.RequestStateHelpers.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Umbrella.AspNetCore.WebUtilities.RequestStateHelpers
{
    public class HttpRequestState : IRequestState
    {
        #region Private Members
        private readonly HttpContext m_Context;
        #endregion

        #region Constructors
        public HttpRequestState(HttpContext context = null) => m_Context = context;
        #endregion

        #region IRequestState Members
        public T Get<T>() where T : class => Get<T>(typeof(T).FullName);

        public T Get<T>(string key) where T : class
            => m_Context.Items.Keys.Contains(key)
                ? m_Context.Items[key] as T
                : null;

        public void Store<T>(T value) => Store(typeof(T).FullName, value);

        public void Store<T>(string key, T value) => m_Context.Items[key] = value;
        #endregion
    }
}