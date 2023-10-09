// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Shared.Constants;
using Umbrella.AppFramework.Shared.Security;
using Umbrella.AppFramework.Shared.Security.Extensions;
using Umbrella.AspNetCore.WebUtilities.Extensions;
using Umbrella.Utilities.Http.Constants;
using Umbrella.Utilities.Primitives;

namespace Umbrella.AspNetCore.WebUtilities.Mvc;

/// <summary>
/// Serves as the base class for API controllers and encapsulates API specific functionality.
/// </summary>
[ApiController]
public abstract class UmbrellaApiController : ControllerBase
{
	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets the hosting environment.
	/// </summary>
	protected IWebHostEnvironment HostingEnvironment { get; }

	/// <summary>
	/// Gets a value indicating whether the application is running in development mode.
	/// </summary>
	protected bool IsDevelopment => HostingEnvironment.IsDevelopment();

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaApiController"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="hostingEnvironment">The hosting environment.</param>
	public UmbrellaApiController(
		ILogger logger,
		IWebHostEnvironment hostingEnvironment)
	{
		Logger = logger;
		HostingEnvironment = hostingEnvironment;

	}

	/// <summary>
	/// Appends the a file access token as a query string parameter with the key <see cref="AppQueryStringKeys.FileAccessToken" /> using the value stored
	/// using the <see cref="UmbrellaAppClaimType.FileAccessToken "/> claim, if it exists on the current <see cref="ClaimsPrincipal"/>.
	/// </summary>
	/// <param name="uri">The URI to append the token to.</param>
	/// <returns>The URI with the appended token if the <see cref="UmbrellaAppClaimType.FileAccessToken" /> claim exists; otherwise the value of the <paramref name="uri"/> parameter.</returns>
	protected string AppendFileAccessToken(string uri)
	{
		string? accessToken = User.GetFileAccessToken();

		return string.IsNullOrWhiteSpace(accessToken) ? uri : QueryHelpers.AddQueryString(uri, AppQueryStringKeys.FileAccessToken, accessToken);
	}

	/// <summary>
	/// Creates a failure result based on the specified <see cref="OperationResult"/>.
	/// </summary>
	/// <param name="operationResult">The operation result.</param>
	/// <returns>The action result.</returns>
	protected IActionResult OperationResultFailure(in OperationResult operationResult)
	{
		switch (operationResult.Status)
		{
			case OperationResultStatus.GenericFailure:
				_ = Logger.WriteError(state: new { operationResult.Status }, message: operationResult.ErrorMessage);
				return BadRequest(operationResult.ErrorMessage);
			case OperationResultStatus.NotFound:
				_ = Logger.WriteWarning(state: new { operationResult.Status }, message: operationResult.ErrorMessage);
				return NotFound(operationResult.ErrorMessage);
			case OperationResultStatus.Conflict:
				_ = Logger.WriteError(state: new { operationResult.Status }, message: operationResult.ErrorMessage);
				return Conflict(operationResult.ErrorMessage);
			default:
				throw new SwitchExpressionException(operationResult.Status);
		}
	}

	/// <summary>
	/// Creates a failure result based on the specified <see cref="OperationResult{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of the item associated with the operation.</typeparam>
	/// <param name="operationResult">The operation result.</param>
	/// <returns>The action result.</returns>
	protected IActionResult OperationResultFailure<T>(in OperationResult<T> operationResult)
	{
		switch (operationResult.Status)
		{
			case OperationResultStatus.GenericFailure:
				_ = Logger.WriteError(state: new { operationResult.Status }, message: operationResult.PrimaryValidationMessage);
				return ValidationProblem(operationResult.ValidationResults.ToModelStateDictionary());
			case OperationResultStatus.NotFound:
				_ = Logger.WriteWarning(state: new { operationResult.Status }, message: operationResult.PrimaryValidationMessage);
				return NotFound(operationResult.PrimaryValidationMessage ?? "Not Found");
			case OperationResultStatus.Conflict:
				_ = Logger.WriteError(state: new { operationResult.Status }, message: operationResult.PrimaryValidationMessage);
				return Conflict(operationResult.PrimaryValidationMessage ?? "Conflict");
			default:
				throw new SwitchExpressionException(operationResult.Status);
		}
	}

	/// <summary>
	/// Creates a failure result based on the specified <see cref="OperationResultException"/>.
	/// </summary>
	/// <param name="exception">The exception.</param>
	/// <returns>The action result.</returns>
	/// <exception cref="SwitchExpressionException"></exception>
	protected IActionResult OperationResultFailure(OperationResultException exception)
	{
		switch(exception.Status)
		{
			case OperationResultStatus.GenericFailure when exception.ValidationResults is not { Count: > 0 }:
				_ = Logger.WriteError(state: new { exception.Status }, message: exception.Message);
				return BadRequest(exception.Message);
			case OperationResultStatus.NotFound when exception.ValidationResults is not { Count: > 0 }:
				_ = Logger.WriteWarning(state: new { exception.Status }, message: exception.Message);
				return NotFound(exception.Message);
			case OperationResultStatus.Conflict when exception.ValidationResults is not { Count: > 0 }:
				_ = Logger.WriteError(state: new { exception.Status }, message: exception.Message);
				return Conflict(exception.Message);
			case OperationResultStatus.GenericFailure when exception.ValidationResults is { Count: > 0 }:
				_ = Logger.WriteError(state: new { exception.Status }, message: exception.PrimaryValidationMessage);
				return ValidationProblem(exception.ValidationResults.ToModelStateDictionary());
			case OperationResultStatus.NotFound when exception.ValidationResults is { Count: > 0 }:
				_ = Logger.WriteWarning(state: new { exception.Status }, message: exception.PrimaryValidationMessage);
				return NotFound(exception.PrimaryValidationMessage ?? "Not Found");
			case OperationResultStatus.Conflict when exception.ValidationResults is { Count: > 0 }:
				_ = Logger.WriteError(state: new { exception.Status }, message: exception.PrimaryValidationMessage);
				return Conflict(exception.PrimaryValidationMessage ?? "Conflict");
			default:
				throw new SwitchExpressionException(exception.Status);
		}
	}

	/// <summary>
	/// Creates a 201 Created <see cref="StatusCodeResult"/>.
	/// </summary>
	/// <returns>A <see cref="StatusCodeResult"/> of 201.</returns>
	protected virtual StatusCodeResult Created() => StatusCode(201);

	/// <summary>
	/// Creates a 201 Created <see cref="CreatedResult"/> with the specified content.
	/// </summary>
	/// <param name="content">The content.</param>
	/// <returns>A <see cref="CreatedResult"/> of 201.</returns>
	protected virtual CreatedResult Created(object content) => Created("", content);

	/// <summary>
	/// Creates a 400 BadRequest <see cref="ObjectResult"/> with the specified reason.
	/// </summary>
	/// <param name="reason">The reason.</param>
	/// <param name="code">The error code.</param>
	/// <returns>A <see cref="ObjectResult"/> of 400.</returns>
	protected virtual ObjectResult BadRequest(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 400, title: "BadRequest", code: code);

	/// <summary>
	/// Creates a 401 Unauthorized <see cref="ObjectResult"/> with the specified reason.
	/// </summary>
	/// <param name="reason">The reason.</param>
	/// <param name="code">The error code.</param>
	/// <returns>A <see cref="ObjectResult"/> of 401.</returns>
	protected virtual ObjectResult Unauthorized(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 401, title: "Unauthorized", code: code);

	/// <summary>
	/// Creates a 403 Forbidden <see cref="ObjectResult"/> with the specified reason.
	/// </summary>
	/// <param name="reason">The reason.</param>
	/// <param name="code">The error code.</param>
	/// <returns>A <see cref="ObjectResult"/> of 403.</returns>
	protected virtual ObjectResult Forbidden(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 403, title: "Forbidden", code: code);

	/// <summary>
	/// Creates a 404 NotFound <see cref="ObjectResult"/> with the specified reason.
	/// </summary>
	/// <param name="reason">The reason.</param>
	/// <param name="code">The error code.</param>
	/// <returns>A <see cref="ObjectResult"/> of 404.</returns>
	protected virtual ObjectResult NotFound(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 404, title: "Not Found", code: code);

	/// <summary>
	/// Creates a 405 MethodNotAllowed <see cref="ObjectResult"/> with the specified reason.
	/// </summary>
	/// <param name="reason">The reason.</param>
	/// <param name="code">The error code.</param>
	/// <returns>A <see cref="ObjectResult"/> of 405.</returns>
	protected virtual ObjectResult MethodNotAllowed(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 405, title: "Method Not Allowed", code: code);

	/// <summary>
	/// Creates a 409 Conflict <see cref="ObjectResult"/> with the specified reason.
	/// </summary>
	/// <param name="reason">The reason.</param>
	/// <param name="code">The error code.</param>
	/// <returns>A <see cref="ObjectResult"/> of 409.</returns>
	protected virtual ObjectResult Conflict(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 409, title: "Conflict", code: code);

	/// <summary>
	/// Creates a 409 Conflict <see cref="ObjectResult"/> with the specified reason with the code set to <see cref="HttpProblemCodes.ConcurrencyStampMismatch"/>.
	/// </summary>
	/// <param name="reason">The reason.</param>
	/// <returns>A <see cref="ObjectResult"/> of 409.</returns>
	protected virtual ObjectResult ConcurrencyConflict(string reason) => UmbrellaProblem(reason, statusCode: 409, title: "Concurrency Conflict", code: HttpProblemCodes.ConcurrencyStampMismatch);

	/// <summary>
	/// Creates a 429 TooManyRequests <see cref="ObjectResult"/> with the specified reason.
	/// </summary>
	/// <param name="reason">The reason.</param>
	/// <param name="code">The error code.</param>
	/// <returns>A <see cref="ObjectResult"/> of 429.</returns>
	protected virtual ObjectResult TooManyRequests(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 429, title: "Too Many Requests", code: code);

	/// <summary>
	/// Creates a 500 InternalServerError <see cref="ObjectResult"/> with the specified reason.
	/// </summary>
	/// <param name="reason">The reason.</param>
	/// <param name="code">The error code.</param>
	/// <returns>A <see cref="ObjectResult"/> of 500.</returns>
	protected virtual ObjectResult InternalServerError(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 500, title: "Error", code: code);

	/// <summary>
	/// Creates an <see cref="ObjectResult"/> containing an <see cref="UmbrellaProblemDetails"/> instance.
	/// </summary>
	/// <param name="detail">The detail.</param>
	/// <param name="instance">The instance.</param>
	/// <param name="statusCode">The status code.</param>
	/// <param name="title">The title.</param>
	/// <param name="type">The type.</param>
	/// <param name="code">The error code.</param>
	/// <returns>The <see cref="ObjectResult"/> containing an instance of <see cref="UmbrellaProblemDetails"/>.</returns>
	protected virtual ObjectResult UmbrellaProblem(string? detail = null, string? instance = null, int? statusCode = null, string? title = null, string? type = null, string? code = null)
	{
		var problemDetails = new UmbrellaProblemDetails
		{
			Code = code,
			Detail = detail,
			Instance = instance,
			Status = statusCode,
			Title = title,
			Type = type,
			CorrelationId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
		};

		return new ObjectResult(problemDetails)
		{
			ContentTypes = { "application/problem+json" },
			StatusCode = statusCode
		};
	}
}