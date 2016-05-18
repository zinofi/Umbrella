using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbrella.WebUtilities.RequestStateHelpers.Interfaces;

namespace Umbrella.Legacy.WebUtilities.RequestStateHelpers
{
	public class OwinRequestState : IRequestState
	{
		public T Get<T>(string key) where T : class
		{
			return HttpContext.Current.GetOwinContext().Get<T>(key);
		}

		public void Store<T>(string key, T something)
		{
			HttpContext.Current.GetOwinContext().Set<T>(key, something);
		}

		public T Get<T>() where T : class
		{
			return Get<T>(typeof(T).FullName);
		}

		public void Store<T>(T something)
		{
			Store(typeof(T).FullName, something);
		}
	}
}
