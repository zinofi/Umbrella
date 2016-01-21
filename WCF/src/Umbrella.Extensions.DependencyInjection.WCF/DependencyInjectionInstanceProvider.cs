using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace Umbrella.Extensions.DependencyInjection.WCF
{
    public class DependencyInjectionInstanceProvider : IInstanceProvider
    {
        #region Private Members
        private readonly IServiceProvider m_RootProvider;
        private readonly Type m_ServiceType;
        #endregion

        #region Constructors
        public DependencyInjectionInstanceProvider(IServiceProvider rootProvider, Type serviceType)
        {
            m_RootProvider = rootProvider;
            m_ServiceType = serviceType;
        }
        #endregion

        #region IInstanceProvider Members
        public object GetInstance(InstanceContext instanceContext) => GetInstance(instanceContext, null);

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            if (instanceContext == null)
            {
                throw new ArgumentNullException(nameof(instanceContext));
            }

            var diContentInstance = new DependencyInjectionInstanceContext(m_RootProvider);
            instanceContext.Extensions.Add(diContentInstance);

            try
            {
                return diContentInstance.Resolve(m_ServiceType);
            }
            catch (Exception)
            {
                diContentInstance.Dispose();
                instanceContext.Extensions.Remove(diContentInstance);
                throw;
            }
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (instanceContext == null)
            {
                throw new ArgumentNullException(nameof(instanceContext));
            }

            var extension = instanceContext.Extensions.Find<DependencyInjectionInstanceContext>();

            if (extension != null)
            {
                extension.Dispose();
            }
        }
        #endregion
    }
}