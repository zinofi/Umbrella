using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Http.Abstractions
{
	/// <summary>
	/// An opinionated generic HTTP service used to query remote endpoints that follow the same conventions.
	/// </summary>
	public interface IGenericHttpService
	{
		/// <summary>
		/// DELETE a resource from the server.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the operation.</returns>
		Task<IHttpCallResult> DeleteAsync(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// GET a resource from the server.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="url">The URL.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the operation.</returns>
		Task<IHttpCallResult<TResult?>> GetAsync<TResult>(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// POST the resource to the server.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="url">The URL.</param>
		/// <param name="item">The item.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the operation.</returns>
		Task<IHttpCallResult> PostAsync<TItem>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// POST the resource to the server.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="url">The URL.</param>
		/// <param name="item">The item.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the operation.</returns>
		Task<IHttpCallResult<TResult?>> PostAsync<TItem, TResult>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// PUT the resource to the server.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="url">The URL.</param>
		/// <param name="item">The item.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the operation.</returns>
		Task<IHttpCallResult> PutAsync<TItem>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// PUT the resource to the server.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="url">The URL.</param>
		/// <param name="item">The item.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the operation.</returns>
		Task<IHttpCallResult<TResult?>> PutAsync<TItem, TResult>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// PATCH the resource to the server.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="url">The URL.</param>
		/// <param name="item">The item.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the operation.</returns>
		Task<IHttpCallResult> PatchAsync<TItem>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// PATCH the resource to the server.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="url">The URL.</param>
		/// <param name="item">The item.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the operation.</returns>
		Task<IHttpCallResult<TResult?>> PatchAsync<TItem, TResult>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// PATCH the resource to the server.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the operation.</returns>
		Task<IHttpCallResult> PatchAsync(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);
	}
}