using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Http.Abstractions
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
		/// <param name="useCache">Determines whether to cache the resource info.</param>
		/// <returns>The <see cref="HttpResourceInfo"/>.</returns>
		Task<HttpResourceInfo?> GetAsync(string url, CancellationToken cancellationToken = default, bool useCache = true);
	}
}