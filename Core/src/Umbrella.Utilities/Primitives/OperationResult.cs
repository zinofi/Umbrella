// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Primitives.Abstractions;

namespace Umbrella.Utilities.Primitives;

/// <summary>
/// Encapsulates the result of an operation.
/// </summary>
public record OperationResult : IOperationResult
{
	public OperationResult(OperationResultStatus status = OperationResultStatus.GenericSuccess)
	{
		Status = status;
	}

	/// <summary>
	/// Creates an <see cref="OperationResult"/> with a <see cref="Status"/> of <see cref="OperationResultStatus.GenericSuccess"/>.
	/// </summary>
	/// <returns>The <see cref="OperationResult"/> instance.</returns>
	public static OperationResult Success() => new();

	/// <summary>
	/// Creates an <see cref="OperationResult"/> with a <see cref="Status"/> of <see cref="OperationResultStatus.GenericFailure"/>.
	/// </summary>
	/// <param name="errorMessage">The error message.</param>
	/// <returns>The <see cref="OperationResult"/> instance.</returns>
	public static OperationResult GenericFailure(string errorMessage) => Failure(OperationResultStatus.GenericFailure, errorMessage);

	/// <summary>
	/// Creates an <see cref="OperationResult"/> with a <see cref="Status"/> of <see cref="OperationResultStatus.GenericFailure"/>.
	/// </summary>
	/// <param name="validationResults">The validation results.</param>
	/// <returns>The <see cref="OperationResult"/> instance.</returns>
	public static OperationResult GenericFailure(IEnumerable<ValidationResult> validationResults) => Failure(OperationResultStatus.GenericFailure, [.. validationResults]);

	/// <summary>
	/// Creates an <see cref="OperationResult"/> with a <see cref="Status"/> of <see cref="OperationResultStatus.NotFound"/>.
	/// </summary>
	/// <param name="errorMessage">The error message.</param>
	/// <returns>The <see cref="OperationResult"/> instance.</returns>
	public static OperationResult NotFound(string errorMessage) => Failure(OperationResultStatus.NotFound, errorMessage);

	/// <summary>
	/// Creates an <see cref="OperationResult"/> with a <see cref="Status"/> of <see cref="OperationResultStatus.Conflict"/>.
	/// </summary>
	/// <param name="errorMessage">The error message.</param>
	/// <returns>The <see cref="OperationResult"/> instance.</returns>
	public static OperationResult Conflict(string errorMessage) => Failure(OperationResultStatus.Conflict, errorMessage);

	public static OperationResult Forbidden(string errorMessage) => Failure(OperationResultStatus.Forbidden, errorMessage);
	public static OperationResult NoContent() => new(OperationResultStatus.NoContent);
	public static OperationResult InvalidOperation(string errorMessage) => Failure(OperationResultStatus.InvalidOperation, errorMessage);

	private static OperationResult Failure(OperationResultStatus status, string errorMessage) => new()
	{
		Status = status,
		ValidationResults = [new ValidationResult(errorMessage)]
	};

	private static OperationResult Failure(OperationResultStatus status, ValidationResult[] validationResults) => new()
	{
		Status = status,
		ValidationResults = validationResults
	};

	/// <inheritdoc />
	public OperationResultStatus Status { get; init; } = OperationResultStatus.GenericSuccess;

	/// <inheritdoc />
	public bool IsSuccess => Status is OperationResultStatus.GenericSuccess or OperationResultStatus.Created or OperationResultStatus.NoContent;

	/// <inheritdoc />
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public IReadOnlyCollection<ValidationResult>? ValidationResults { get; init; }

	/// <inheritdoc />
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? PrimaryValidationMessage => ValidationResults?.FirstOrDefault()?.ErrorMessage;
}

/// <summary>
/// Encapsulates the result of an operation with a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public record OperationResult<TResult> : OperationResult, IOperationResult<TResult>
{
	/// <summary>
	/// Creates an <see cref="OperationResult{TResult}"/> with a <see cref="OperationResult.Status"/> of <see cref="OperationResultStatus.GenericSuccess"/> for the specified <paramref name="item"/>.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <returns>The <see cref="OperationResult{TResult}"/> instance.</returns>
	public static OperationResult<TResult> Success(TResult item) => new()
	{
		Status = OperationResultStatus.GenericSuccess,
		Result = item,
	};

	/// <summary>
	/// Creates an <see cref="OperationResult{TResult}"/> with a <see cref="OperationResult.Status"/> of <see cref="OperationResultStatus.Created"/> for the specified <paramref name="item"/>.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <returns>The <see cref="OperationResult{TResult}"/> instance.</returns>
	public static OperationResult<TResult> Created(TResult item) => new()
	{
		Status = OperationResultStatus.Created,
		Result = item,
	};

	/// <summary>
	/// Creates an <see cref="OperationResult{TResult}"/> with a <see cref="OperationResult.Status"/> of <see cref="OperationResultStatus.GenericFailure"/> for the specified <paramref name="item"/>.
	/// </summary>
	/// <param name="errorMessage">The error message.</param>
	/// <param name="item">The item.</param>
	/// <returns>The <see cref="OperationResult{TResult}"/> instance.</returns>
	public static OperationResult<TResult> GenericFailure(string errorMessage, TResult? item = default) => Failure(OperationResultStatus.GenericFailure, item, [new ValidationResult(errorMessage)]);

	/// <summary>
	/// Creates an <see cref="OperationResult{TResult}"/> with a <see cref="OperationResult.Status"/> of <see cref="OperationResultStatus.GenericFailure"/> for the specified <paramref name="result"/>.
	/// </summary>
	/// <param name="validationResults">The validation results.</param>
	/// <param name="result">The result.</param>
	/// <returns>The <see cref="OperationResult{TResult}"/> instance.</returns>
	public static OperationResult<TResult> GenericFailure(IEnumerable<ValidationResult> validationResults, TResult? result = default) => Failure(OperationResultStatus.GenericFailure, result, [.. validationResults]);

	/// <summary>
	/// Creates an <see cref="OperationResult{TResult}"/> with a <see cref="OperationResult.Status"/> of <see cref="OperationResultStatus.NotFound"/> for the specified <paramref name="result"/>.
	/// </summary>
	/// <param name="errorMessage">The error message.</param>
	/// <param name="result">The result.</param>
	/// <returns>The <see cref="OperationResult{TResult}"/> instance.</returns>
	public static OperationResult<TResult> NotFound(string errorMessage, TResult? result = default) => Failure(OperationResultStatus.NotFound, result, [new ValidationResult(errorMessage)]);

	/// <summary>
	/// Creates an <see cref="OperationResult{TResult}"/> with a <see cref="OperationResult.Status"/> of <see cref="OperationResultStatus.Conflict"/> for the specified <paramref name="result"/>.
	/// </summary>
	/// <param name="errorMessage">The error message.</param>
	/// <param name="result">The result.</param>
	/// <returns>The <see cref="OperationResult{TResult}"/> instance.</returns>
	public static OperationResult<TResult> Conflict(string errorMessage, TResult? result = default) => Failure(OperationResultStatus.Conflict, result, [new ValidationResult(errorMessage)]);

	private static OperationResult<TResult> Failure(OperationResultStatus status, TResult? result, ValidationResult[] validationResults) => new()
	{
		Result = result,
		Status = status,
		ValidationResults = validationResults
	};

	/// <inheritdoc />
	public TResult? Result { get; init; }
}

/// <summary>
/// A status for an <see cref="OperationResult"/> or <see cref="OperationResult{T}"/>.
/// </summary>
public enum OperationResultStatus
{
	/// <summary>
	/// This is the default value and indicates that the operation was successful.
	/// </summary>
	GenericSuccess = 0, // Same as HTTP 200 OK. Generic success status for operations.

	/// <summary>
	/// Indicates that the operation failed with a non-specific error.
	/// </summary>
	GenericFailure = 500,

    /// <summary>
    /// Indicates that the operation failed because the target of the operation could not be found.
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// Indicates that the operation failed because the target of the operation conflicts with something,
    /// e.g, when creating a user, another user already exists with the same username.
    /// </summary>
    Conflict = 409,

	Forbidden = 403,

	NoContent = 204,

	InvalidOperation = 400,

	Created = 201
}

/// <summary>
/// An exception that encapsulates the state of an <see cref="OperationResult" />.
/// </summary>
/// <seealso cref="UmbrellaException" />
[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "This is a specialized exception.")]
public class OperationResultException : UmbrellaException
{
    /// <summary>
    /// Gets the status.
    /// </summary>
    public OperationResultStatus Status { get; init; }

    /// <summary>
    /// Gets the validation results.
    /// </summary>
    public IReadOnlyCollection<ValidationResult> ValidationResults { get; init; } = Array.Empty<ValidationResult>();

    /// <summary>
    /// Gets the primary validation message which is the first message in the <see cref="ValidationResults"/> collection.
    /// </summary>
    public string? PrimaryValidationMessage => ValidationResults.FirstOrDefault()?.ErrorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResultException"/> class.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <param name="validationResults">The validation results.</param>
    public OperationResultException(OperationResultStatus status, IReadOnlyCollection<ValidationResult>? validationResults = null)
    {
        Status = status;
        ValidationResults = validationResults ?? Array.Empty<ValidationResult>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResultException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="status">The status.</param>
    /// <param name="validationResults">The validation results.</param>
    public OperationResultException(string message, OperationResultStatus status, IReadOnlyCollection<ValidationResult>? validationResults = null) : base(message)
    {
        Status = status;
        ValidationResults = validationResults ?? Array.Empty<ValidationResult>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResultException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="status">The status.</param>
    /// <param name="validationResults">The validation results.</param>
    public OperationResultException(string message, Exception innerException, OperationResultStatus status, IReadOnlyCollection<ValidationResult>? validationResults = null) : base(message, innerException)
    {
        Status = status;
        ValidationResults = validationResults ?? Array.Empty<ValidationResult>();
    }
}