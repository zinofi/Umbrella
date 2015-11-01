using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Ninject
{
	public class NinjectMvcDependencyResolver : IDependencyResolver
	{
		private IKernel m_Kernel;

		public NinjectMvcDependencyResolver(IKernel kernel)
		{
			m_Kernel = kernel;
		}

		public object GetService(Type serviceType)
		{
			return m_Kernel.TryGet(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return m_Kernel.GetAll(serviceType);
		}
	}
}
