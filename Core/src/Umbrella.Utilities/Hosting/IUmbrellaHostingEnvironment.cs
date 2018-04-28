using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Hosting
{
	public interface IUmbrellaHostingEnvironment
	{
		string MapPath(string virtualPath, bool fromContentRoot = true);
        string GetFileContent(string virtualPath, bool fromContentRoot = true, bool cache = true, bool watch = true);
	}
}