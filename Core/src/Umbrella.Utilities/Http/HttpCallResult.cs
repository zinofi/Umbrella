using Umbrella.Utilities.Http.Abstractions;

namespace Umbrella.Utilities.Http;

/// <summary>
/// Represents the result of a Http call.
/// </summary>
/// <seealso cref="IHttpCallResult"/>
[Obsolete("To be removed", true)]
public class HttpCallResult : IHttpCallResult
{
	/// <summary>
	/// Initializes a new instance of the <see cref="HttpCallResult"/> class.
	/// </summary>
	/// <param name="success">if set to <c>true</c> signals that the call succeeded without errors.</param>
	/// <param name="problemDetails">The problem details.</param>
	public HttpCallResult(bool success, HttpProblemDetails? problemDetails = null)
	{
		Success = success;
		ProblemDetails = problemDetails;
	}

	/// <inheritdoc />
	public bool Success { get; }

	/// <inheritdoc />
	public HttpProblemDetails? ProblemDetails { get; }
}

/// <summary>
/// Represents the result of a Http call.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <seealso cref="IHttpCallResult{TResult}"/>
[Obsolete("To be removed", true)]
public class HttpCallResult<TResult> : HttpCallResult, IHttpCallResult<TResult>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="HttpCallResult{TResult}"/> class.
	/// </summary>
	/// <param name="success">if set to <c>true</c> signals that the call succeeded without errors.</param>
	/// <param name="problemDetails">The problem details.</param>
	/// <param name="result">The result.</param>
	public HttpCallResult(bool success, HttpProblemDetails? problemDetails = null, TResult result = default!)
		: base(success, problemDetails)
	{
		Result = result!;
	}

	/// <inheritdoc />
	public TResult Result { get; }
}