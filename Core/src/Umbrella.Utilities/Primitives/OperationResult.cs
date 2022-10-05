// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Primitives;

/// <summary>
/// Encapsulates the result of an operation.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct OperationResult
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OperationResult"/> struct.
	/// </summary>
	public OperationResult()
	{
	}

	public static OperationResult Success() => new();
	public static OperationResult GenericFailure(string errorMessage) => Failure(OperationResultStatus.GenericFailure, errorMessage);
	public static OperationResult NotFound(string errorMessage) => Failure(OperationResultStatus.NotFound, errorMessage);
	public static OperationResult Conflict(string errorMessage) => Failure(OperationResultStatus.Conflict, errorMessage);

	private static OperationResult Failure(OperationResultStatus status, string errorMessage) => new()
	{
		Status = status,
		ErrorMessage = errorMessage
	};

	/// <summary>
	/// Gets the status of the operation.
	/// </summary>
	/// <remarks>Defaults to <see cref="OperationResultStatus.Success"/></remarks>
	public OperationResultStatus Status { get; init; } = OperationResultStatus.Success;

	/// <summary>
	/// Gets the error message.
	/// </summary>
	public string ErrorMessage { get; init; } = "";
}

/// <summary>
/// Encapsulates the result of an operation with a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly struct OperationResult<TResult>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OperationResult{T}"/> struct.
	/// </summary>
	public OperationResult()
	{
	}

	public static OperationResult<TResult> Success(TResult item) => new()
	{
		Status = OperationResultStatus.Success,
		Result = item,
	};

	public static OperationResult<TResult> GenericFailure(string errorMessage, TResult? item = default) => Failure(OperationResultStatus.GenericFailure, item, new[] { new ValidationResult(errorMessage) });
	public static OperationResult<TResult> GenericFailure(IEnumerable<ValidationResult> validationResults, TResult? result = default) => Failure(OperationResultStatus.GenericFailure, result, validationResults.ToArray());
	public static OperationResult<TResult> NotFound(string errorMessage, TResult? result = default) => Failure(OperationResultStatus.NotFound, result, new[] { new ValidationResult(errorMessage) });
	public static OperationResult<TResult> Conflict(string errorMessage, TResult? result = default) => Failure(OperationResultStatus.Conflict, result, new[] { new ValidationResult(errorMessage) });

	private static OperationResult<TResult> Failure(OperationResultStatus status, TResult? result, ValidationResult[] validationResults) => new()
	{
		Result = result,
		Status = status,
		ValidationResults = validationResults
	};

	/// <summary>
	/// The item that the associated operation was being performed on.
	/// </summary>
	public TResult? Result { get; init; } = default;

	/// <summary>
	/// Gets the status of the operation.
	/// </summary>
	/// <remarks>Defaults to <see cref="OperationResultStatus.Success"/></remarks>
	public OperationResultStatus Status { get; init; } = OperationResultStatus.Success;

	/// <summary>
	/// A list of validation results that contain messages detailing why it might be the case that the operation could not be completed.
	/// </summary>
	public IReadOnlyCollection<ValidationResult> ValidationResults { get; init; } = Array.Empty<ValidationResult>();

	/// <summary>
	/// Gets the primary validation message which is the first message in the <see cref="ValidationResults"/> collection.
	/// </summary>
	public string? PrimaryValidationMessage => ValidationResults.FirstOrDefault()?.ErrorMessage;
}

/// <summary>
/// A status for an <see cref="OperationResult"/> or <see cref="OperationResult{T}"/>.
/// </summary>
public enum OperationResultStatus
{
	Success = 0,
	GenericFailure = 1,
	NotFound = 2,
	Conflict = 3
}