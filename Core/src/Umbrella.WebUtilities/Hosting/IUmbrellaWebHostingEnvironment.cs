using System.Collections.Generic;
using Umbrella.Utilities.Hosting.Abstractions;

namespace Umbrella.WebUtilities.Hosting
{
	public interface IUmbrellaWebHostingEnvironment : IUmbrellaHostingEnvironment
	{
		string MapWebPath(string virtualPath, bool toAbsoluteUrl = false, string scheme = "http", bool appendVersion = false, string versionParameterName = "v", bool mapFromContentRoot = true, bool watchWhenAppendVersion = true);
	}
}