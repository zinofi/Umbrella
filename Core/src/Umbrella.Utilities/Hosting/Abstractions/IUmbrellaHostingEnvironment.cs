using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Hosting.Abstractions
{
	public interface IUmbrellaHostingEnvironment
	{
		// TODO: Need to remove the fromContentRoot parameter from these methods and create new methods on the web interfaces.
		// Needs a bit of work though!
		string MapPath(string virtualPath, bool fromContentRoot = true);
		Task<string> GetFileContentAsync(string virtualPath, bool fromContentRoot = true, bool cache = true, bool watch = true, CancellationToken cancellationToken = default);
	}
}