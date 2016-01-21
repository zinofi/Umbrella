using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;

namespace Umbrella.Extensions.DependencyInjection.WCF
{
    public abstract class DependencyInjectionServiceHostFactory : ServiceHostFactory
    {
        #region Private Members
        private readonly IServiceCollection m_ServiceContainer = new ServiceCollection();
        #endregion

        #region ServiceHostFactory Members
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            //Register the service as a type so it can be found from the instance provider
            m_ServiceContainer.AddTransient(serviceType);

            RegisterDependencies(m_ServiceContainer);

            ServiceHost host = new ServiceHost(serviceType, baseAddresses);
            host.Opening += (sender, args) => host.Description.Behaviors.Add(new DependencyInjectionServiceBehavior(m_ServiceContainer.BuildServiceProvider()));

            return host;
        }
        #endregion

        #region Public Abstract Methods
        protected abstract void RegisterDependencies(IServiceCollection services);
        #endregion
    }
}