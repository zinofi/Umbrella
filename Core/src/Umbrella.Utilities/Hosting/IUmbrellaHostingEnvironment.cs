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
		string MapWebPath(string virtualPath, bool toAbsoluteUrl = false, string scheme = "http", bool appendVersion = false, string versionParameterName = "v", bool mapFromContentRoot = true);
	}
}