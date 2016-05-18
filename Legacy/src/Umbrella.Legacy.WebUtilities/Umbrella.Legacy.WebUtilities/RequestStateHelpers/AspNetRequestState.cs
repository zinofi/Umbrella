using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Umbrella.WebUtilities.RequestStateHelpers.Interfaces;

namespace Umbrella.Legacy.WebUtilities.RequestStateHelpers
{
    public class AspNetRequestState : IRequestState
    {
        #region IRequestState Members
        public T Get<T>() where T : class
        {
            return Get<T>(typeof(T).FullName);
        }

        public T Get<T>(string key) where T : class
        {
            return HttpContext.Current.Items[key] as T;
        }

        public void Store<T>(T something)
        {
            Store(typeof(T).FullName, something);
        }

        public void Store<T>(string key, T something)
        {
            if (HttpContext.Current.Items.Contains(key))
                HttpContext.Current.Items[key] = something;
            else
                HttpContext.Current.Items.Add(key, something);
        }
        #endregion
    }
}
