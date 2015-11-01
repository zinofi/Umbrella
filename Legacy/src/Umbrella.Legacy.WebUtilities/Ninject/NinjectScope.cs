using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;

namespace Umbrella.Legacy.WebUtilities.Ninject
{
	public class NinjectScope : IDependencyScope
	{
		protected IResolutionRoot m_ResolutionRoot;

		public NinjectScope(IResolutionRoot kernel)
		{
			m_ResolutionRoot = kernel;
		}
		public object GetService(Type serviceType)
		{
			IRequest request = m_ResolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
			return m_ResolutionRoot.Resolve(request).SingleOrDefault();
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			IRequest request = m_ResolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
			return m_ResolutionRoot.Resolve(request).ToList();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(m_ResolutionRoot != null)
				{
					IDisposable disposable = m_ResolutionRoot as IDisposable;
					if(disposable != null)
					{
						disposable.Dispose();
						m_ResolutionRoot = null;
					}
				}
			}
		}
	}
}