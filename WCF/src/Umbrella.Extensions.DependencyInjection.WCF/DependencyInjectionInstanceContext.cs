using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Umbrella.Extensions.DependencyInjection.WCF
{
    public class DependencyInjectionInstanceContext : IExtension<InstanceContext>, IDisposable
    {
        #region Private Members
        private bool m_Disposed;
        private readonly IServiceScope m_ServiceScope;
        #endregion

        #region Constructors
        public DependencyInjectionInstanceContext(IServiceProvider rootProvider)
        {
            if (rootProvider == null)
            {
                throw new ArgumentNullException(nameof(rootProvider));
            }

            m_ServiceScope = rootProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }
        #endregion

        #region Finalizer
        ~DependencyInjectionInstanceContext()
        {
            Dispose(false);
        }
        #endregion

        #region IExtension Members
        public void Attach(InstanceContext owner)
        {
        }

        public void Detach(InstanceContext owner)
        {
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Public Methods
        public object Resolve(Type serviceType) => m_ServiceScope.ServiceProvider.GetService(serviceType);
        #endregion

        #region Private Methods
        private void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    // Free managed resources
                    m_ServiceScope.Dispose();
                }

                m_Disposed = true;
            }
        }
        #endregion
    }
}