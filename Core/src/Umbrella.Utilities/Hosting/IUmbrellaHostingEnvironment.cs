using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Hosting
{
	public interface IUmbrellaHostingEnvironment
	{
		string MapPath(string virtualPath);
		string MapWebPath(string virtualPath, string scheme = "http");
	}
}