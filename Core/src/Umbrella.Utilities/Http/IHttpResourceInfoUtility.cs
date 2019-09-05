using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Http
{
	/// <summary>
	/// A utility class used to get basic details of a resource on a URL.
	/// </summary>
	public interface IHttpResourceInfoUtility
	{
		/// <summary>
		/// Gets the <see cref="HttpResourceInfo"/> for the specified <paramref name="url"/>. Returns null where the resource cannot be found.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="useCache">if set to  [use cache].</param>
		/// <returns></returns>
		/// <exception cref="UmbrellaException">There was a problem retrieving data for the specified url: {url}</exception>
		Task<HttpResourceInfo> GetAsync(string url, CancellationToken cancellationToken = default, bool useCache = true);
	}
}