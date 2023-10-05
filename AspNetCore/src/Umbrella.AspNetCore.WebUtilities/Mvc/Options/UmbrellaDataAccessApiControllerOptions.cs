// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Umbrella.AspNetCore.WebUtilities.Security.Policies;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.Options;

/// <summary>
/// Options for use with the <see cref="UmbrellaDataAccessApiController"/>
/// </summary>
/// <seealso cref="ISanitizableUmbrellaOptions" />
/// <seealso cref="IValidatableUmbrellaOptions" />
public class UmbrellaDataAccessApiControllerOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the concurrency error message returned in <see cref="UmbrellaApiController.ConcurrencyConflict"/> (status code 409) responses.
	/// </summary>
	public string ConcurrencyErrorMessage { get; set; } = "This information has been changed elsewhere since this screen was loaded. Please try again.";

	/// <summary>
	/// Gets or sets the exception filter that is invoked when exceptions are thrown inside the <see cref="UmbrellaDataAccessApiController.ReadAllAsync"/> method.
	/// </summary>
	/// <remarks>
	/// If the exception filter returns <see langword="true"/>, the <see cref="HandleReadAllExceptionAsync"/> delegate is invoked, if it has been specified.
	/// </remarks>
	public Func<Exception, bool> ReadAllExceptionFilter { get; set; } = _ => false;

	/// <summary>
	/// Gets or sets the delegate used to handle exceptions where the <see cref="ReadAllExceptionFilter"/> returns <see langword="true"/>.
	/// If this is set to <see langword="null"/>, the exception is simply re-thrown.
	/// </summary>
	public Func<Exception, Task<IActionResult?>> HandleReadAllExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);

	/// <summary>
	/// Gets or sets the exception filter that is invoked when exceptions are thrown inside the <see cref="UmbrellaDataAccessApiController.ReadAsync"/> method.
	/// </summary>
	/// <remarks>
	/// If the exception filter returns <see langword="true"/>, the <see cref="HandleReadExceptionAsync"/> delegate is invoked, if it has been specified.
	/// </remarks>
	public Func<Exception, bool> ReadExceptionFilter { get; set; } = _ => false;

	/// <summary>
	/// Gets or sets the delegate used to handle exceptions where the <see cref="ReadExceptionFilter"/> returns <see langword="true"/>.
	/// If this is set to <see langword="null"/>, the exception is simply re-thrown.
	/// </summary>
	public Func<Exception, Task<IActionResult?>> HandleReadExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);

	/// <summary>
	/// Gets or sets the exception filter that is invoked when exceptions are thrown inside the <see cref="UmbrellaDataAccessApiController.CreateAsync"/> method.
	/// </summary>
	/// <remarks>
	/// If the exception filter returns <see langword="true"/>, the <see cref="HandleCreateExceptionAsync"/> delegate is invoked, if it has been specified.
	/// </remarks>
	public Func<Exception, bool> CreateExceptionFilter { get; set; } = _ => false;

	/// <summary>
	/// Gets or sets the delegate used to handle exceptions where the <see cref="CreateExceptionFilter"/> returns <see langword="true"/>.
	/// If this is set to <see langword="null"/>, the exception is simply re-thrown.
	/// </summary>
	public Func<Exception, Task<IActionResult?>> HandleCreateExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);

	/// <summary>
	/// Gets or sets the exception filter that is invoked when exceptions are thrown inside the <see cref="UmbrellaDataAccessApiController.UpdateAsync"/> method.
	/// </summary>
	/// <remarks>
	/// If the exception filter returns <see langword="true"/>, the <see cref="HandleUpdateExceptionAsync"/> delegate is invoked, if it has been specified.
	/// </remarks>
	public Func<Exception, bool> UpdateExceptionFilter { get; set; } = _ => false;

	/// <summary>
	/// Gets or sets the delegate used to handle exceptions where the <see cref="UpdateExceptionFilter"/> returns <see langword="true"/>.
	/// If this is set to <see langword="null"/>, the exception is simply re-thrown.
	/// </summary>
	public Func<Exception, Task<IActionResult?>> HandleUpdateExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);

	/// <summary>
	/// Gets or sets the exception filter that is invoked when exceptions are thrown inside the <see cref="UmbrellaDataAccessApiController.DeleteAsync"/> method.
	/// </summary>
	/// <remarks>
	/// If the exception filter returns <see langword="true"/>, the <see cref="HandleDeleteExceptionAsync"/> delegate is invoked, if it has been specified.
	/// </remarks>
	public Func<Exception, bool> DeleteExceptionFilter { get; set; } = _ => false;

	/// <summary>
	/// Gets or sets the delegate used to handle exceptions where the <see cref="DeleteExceptionFilter"/> returns <see langword="true"/>.
	/// If this is set to <see langword="null"/>, the exception is simply re-thrown.
	/// </summary>
	public Func<Exception, Task<IActionResult?>> HandleDeleteExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);

	/// <summary>
	/// Gets or sets the name of the authorization policy checked when creating items.
	/// </summary>
	public string CreatePolicyName { get; set; } = CorePolicyNames.Create;

	/// <summary>
	/// Gets or sets the name of the authorization policy checked when reading items.
	/// </summary>
	public string ReadPolicyName { get; set; } = CorePolicyNames.Read;

	/// <summary>
	/// Gets or sets the name of the authorization policy checked when updating items.
	/// </summary>
	public string UpdatePolicyName { get; set; } = CorePolicyNames.Update;

	/// <summary>
	/// Gets or sets the name of the authorization policy checked when deleting items.
	/// </summary>
	public string DeletePolicyName { get; set; } = CorePolicyNames.Delete;

	/// <inheritdoc />
	public void Sanitize()
	{
		ConcurrencyErrorMessage = ConcurrencyErrorMessage?.Trim()!;
		CreatePolicyName = CreatePolicyName?.Trim()!;
		ReadPolicyName = ReadPolicyName?.Trim()!;
		UpdatePolicyName = UpdatePolicyName?.Trim()!;
		DeletePolicyName = DeletePolicyName?.Trim()!;
	}

	/// <inheritdoc />
	public void Validate()
	{
		Guard.IsNotNullOrEmpty(ConcurrencyErrorMessage);
		Guard.IsNotNullOrEmpty(CreatePolicyName);
		Guard.IsNotNullOrEmpty(ReadPolicyName);
		Guard.IsNotNullOrEmpty(UpdatePolicyName);
		Guard.IsNotNullOrEmpty(DeletePolicyName);
	}
}