using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Umbrella.WebUtilities.RequestStateHelpers.Core
{
    internal class DataExtension : IExtension<OperationContext>, IDisposable
    {
        #region Public Properties
        public IDictionary<string, object> State { get; private set; }
        #endregion

        #region Constructors
        public DataExtension()
        {
            State = new Dictionary<string, object>();
        }
        #endregion

        #region IExtension<OperationContext> Members
        // we don't really need implementations for these methods in this case
        public void Attach(OperationContext owner) { }
        public void Detach(OperationContext owner) { }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            foreach (IDisposable disposable in State.Values.OfType<IDisposable>())
                disposable.Dispose();
        }
        #endregion
    }
}
