﻿using System.ComponentModel.DataAnnotations;

namespace Umbrella.Utilities.Http;

/// <summary>
/// A custom Http ProblemDetails type that replicates the ProblemDetails type from ASP.NET Core
/// </summary>
/// <remarks>
/// A machine-readable format for specifying errors in HTTP API responses based on
/// https://tools.ietf.org/html/rfc7807.
/// </remarks>
public class HttpProblemDetails
{
	/// <summary>
	/// A short, human-readable summary of the problem type.It SHOULD NOT change from occurrence to occurrence
	/// of the problem, except for purposes of localization(e.g., using proactive content negotiation;
	/// see[RFC7231], Section 3.4).
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// A human-readable explanation specific to this occurrence of the problem.
	/// </summary>
	public string? Detail { get; set; }

	/// <summary>
	/// A URI reference [RFC3986] that identifies the problem type. This specification encourages that, when
	/// dereferenced, it provide human-readable documentation for the problem type
	/// (e.g., using HTML [W3C.REC-html5-20141028]).  When this member is not present, its value is assumed to be
	/// "about:blank".
	/// </summary>
	public string? Type { get; set; }

	/// <summary>
	/// A URI reference that identifies the specific occurrence of the problem. It may or may not yield further information if dereferenced.
	/// </summary>
	public string? Instance { get; set; }

	/// <summary>
	/// The HTTP status code([RFC7231], Section 6) generated by the origin server for this occurrence of the problem.
	/// </summary>
	public int? Status { get; set; }

	/// <summary>
	/// Gets or sets the errors.
	/// </summary>
	public IDictionary<string, string[]>? Errors { get; set; }

	/// <summary>
	/// Gets or sets a specific error code which can be used by the client
	/// to handle the error more precisely.
	/// </summary>
	public string? Code { get; set; }

	/// <summary>
	/// Gets or sets the correlation id that can be used to identify the details
	/// associated with this problem in logs.
	/// </summary>
	public string? CorrelationId { get; set; }

	/// <summary>
	/// Converts the <see cref="Errors"/> to a collection of <see cref="ValidationResult"/> instances.
	/// </summary>
	/// <returns>The <see cref="ValidationResult"/> collection.</returns>
	public IReadOnlyCollection<ValidationResult> ToValidationResults() => Errors?.Count > 0
			? Errors.SelectMany(x => x.Value.Select(y => new ValidationResult(y, new[] { x.Key }))).ToArray()
			: [];
}