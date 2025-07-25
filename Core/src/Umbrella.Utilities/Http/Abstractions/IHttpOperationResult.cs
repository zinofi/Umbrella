using Umbrella.Utilities.Primitives.Abstractions;

namespace Umbrella.Utilities.Http.Abstractions;

/// <summary>
/// Represents the result of a Http operation.
/// </summary>
public interface IHttpOperationResult : IOperationResult
{
	/// <summary>
	/// Gets the problem details.
	/// </summary>
	HttpProblemDetails? ProblemDetails { get; }
}

/// <summary>
/// Represents the result of a Http operation with a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IHttpOperationResult<out TResult> : IHttpOperationResult, IOperationResult<TResult>
{
}