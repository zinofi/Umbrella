using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Umbrella.WebUtilities.RequestStateHelpers.Core;
using Umbrella.WebUtilities.RequestStateHelpers.Interfaces;

namespace Umbrella.WebUtilities.RequestStateHelpers
{
    public class WcfRequestState : IRequestState
    {
        #region Private Static Members
        private static IDictionary<string, object> State
        {
            get
            {
                DataExtension extension = OperationContext.Current.Extensions.Find<DataExtension>();

                if (extension == null)
                {
                    extension = new DataExtension();
                    OperationContext.Current.Extensions.Add(extension);
                }

                return extension.State;
            }
        }
        #endregion

        #region IRequestState Members
        public T Get<T>(string key) where T : class
        {
            return State.Keys.Contains(key)
                ? State[key] as T
                : null;
        }

        public T Get<T>() where T : class
        {
            return Get<T>(typeof(T).FullName);
        }

        public void Store<T>(string key, T value)
        {
            State[key] = value;
        }

        public void Store<T>(T value)
        {
            Store(typeof(T).FullName, value);
        }
        #endregion
    }
}
