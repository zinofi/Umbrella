using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;

namespace Umbrella.Legacy.WebUtilities.Ninject
{
	public class NinjectWebApiDependencyResolver : NinjectScope, IDependencyResolver
	{
		private readonly IKernel m_Kernel;

		public NinjectWebApiDependencyResolver(IKernel kernel)
			: base(kernel)
		{
			m_Kernel = kernel;
		}

		public IDependencyScope BeginScope()
		{
			return new NinjectScope(m_Kernel.BeginBlock());
		}
	}
}
