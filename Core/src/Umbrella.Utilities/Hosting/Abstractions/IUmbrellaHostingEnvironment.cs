using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Hosting.Abstractions
{
	public interface IUmbrellaHostingEnvironment
	{
		string MapPath(string virtualPath, bool fromContentRoot = true);
		Task<string> GetFileContentAsync(string virtualPath, bool fromContentRoot = true, bool cache = true, bool watch = true, CancellationToken cancellationToken = default);
	}
}