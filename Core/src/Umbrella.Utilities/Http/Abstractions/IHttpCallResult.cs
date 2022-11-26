namespace Umbrella.Utilities.Http.Abstractions;

/// <summary>
/// Represents the result of a Http call.
/// </summary>
public interface IHttpCallResult
{
	/// <summary>
	/// Gets the problem details.
	/// </summary>
	HttpProblemDetails? ProblemDetails { get; }

	/// <summary>
	/// Gets a value indicating whether this <see cref="IHttpCallResult"/> has been a success.
	/// </summary>
	bool Success { get; }
}

/// <summary>
/// Represents the result of a Http call.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IHttpCallResult<out TResult> : IHttpCallResult
{
	/// <summary>
	/// Gets the result.
	/// </summary>
	TResult Result { get; }
}