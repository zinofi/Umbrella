namespace Umbrella.Utilities.Http
{
	/// <summary>
	/// Represents the result of a Http call.
	/// </summary>
	public class HttpCallResult
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HttpCallResult"/> class.
		/// </summary>
		/// <param name="success">if set to <c>true</c> signals that the call succeeded without errors.</param>
		/// <param name="problemDetails">The problem details.</param>
		public HttpCallResult(bool success, HttpProblemDetails problemDetails = null)
		{
			Success = success;
			ProblemDetails = problemDetails;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="HttpCallResult"/> has been a success.
		/// </summary>
		public bool Success { get; }

		/// <summary>
		/// Gets the problem details.
		/// </summary>
		public HttpProblemDetails ProblemDetails { get; }
	}

	/// <summary>
	/// Represents the result of a Http call.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	public class HttpCallResult<TResult> : HttpCallResult // TODO: Create a covariant interface for this to simply shared result assignment.
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HttpCallResult{TResult}"/> class.
		/// </summary>
		/// <param name="success">if set to <c>true</c> signals that the call succeeded without errors.</param>
		/// <param name="problemDetails">The problem details.</param>
		/// <param name="result">The result.</param>
		public HttpCallResult(bool success, HttpProblemDetails problemDetails = null, TResult result = default)
			: base(success, problemDetails)
		{
			Result = result;
		}

		/// <summary>
		/// Gets the result.
		/// </summary>
		public TResult Result { get; }
	}
}