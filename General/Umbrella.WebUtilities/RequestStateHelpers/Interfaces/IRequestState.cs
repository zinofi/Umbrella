using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.RequestStateHelpers.Interfaces
{
	public interface IRequestState
	{
		T Get<T>(string key) where T : class;
		void Store<T>(string key, T value);
		T Get<T>() where T : class;
		void Store<T>(T value);
	}
}
