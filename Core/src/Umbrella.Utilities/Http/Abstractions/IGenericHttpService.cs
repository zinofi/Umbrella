namespace Umbrella.Utilities.Http.Abstractions;

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
	Task<IHttpOperationResult> DeleteAsync(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// GET a resource from the server.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="url">The URL.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the operation.</returns>
	Task<IHttpOperationResult<TResult?>> GetAsync<TResult>(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
		where TResult : class;

	/// <summary>
	/// POST the resource to the server.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <param name="url">The URL.</param>
	/// <param name="item">The item.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the operation.</returns>
	Task<IHttpOperationResult> PostAsync<TItem>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

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
	Task<IHttpOperationResult<TResult?>> PostAsync<TItem, TResult>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
		where TResult : class;

	/// <summary>
	/// PUT the resource to the server.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <param name="url">The URL.</param>
	/// <param name="item">The item.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the operation.</returns>
	Task<IHttpOperationResult> PutAsync<TItem>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

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
	Task<IHttpOperationResult<TResult?>> PutAsync<TItem, TResult>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
		where TResult : class;

	/// <summary>
	/// PATCH the resource to the server.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <param name="url">The URL.</param>
	/// <param name="item">The item.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the operation.</returns>
	Task<IHttpOperationResult> PatchAsync<TItem>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

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
	Task<IHttpOperationResult<TResult?>> PatchAsync<TItem, TResult>(string url, TItem item, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
		where TResult : class;

	/// <summary>
	/// PATCH the resource to the server.
	/// </summary>
	/// <param name="url">The URL.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the operation.</returns>
	Task<IHttpOperationResult> PatchAsync(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// PATCH the resource to the server.
	/// </summary>
	/// <param name="url">The URL.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The result of the operation.</returns>
	Task<IHttpOperationResult<TResult?>> PatchAsync<TResult>(string url, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken cancellationToken = default)
		where TResult : class;
}