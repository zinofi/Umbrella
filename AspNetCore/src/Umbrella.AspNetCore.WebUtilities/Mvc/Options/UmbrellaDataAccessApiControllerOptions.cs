// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Umbrella.AspNetCore.WebUtilities.Security.Policies;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.Options;

public class UmbrellaDataAccessApiControllerOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	public string ConcurrencyErrorMessage { get; set; } = "This information has been changed elsewhere since this screen was loaded. Please try again.";
	public Func<Exception, bool> ReadAllExceptionFilter { get; set; } = _ => false;
	public Func<Exception, Task<IActionResult?>> HandleReadAllExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);
	public Func<Exception, bool> ReadExceptionFilter { get; set; } = _ => false;
	public Func<Exception, Task<IActionResult?>> HandleReadExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);
	public Func<Exception, bool> CreateExceptionFilter { get; set; } = _ => false;
	public Func<Exception, Task<IActionResult?>> HandleCreateExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);
	public Func<Exception, bool> UpdateExceptionFilter { get; set; } = _ => false;
	public Func<Exception, Task<IActionResult?>> HandleUpdateExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);
	public Func<Exception, bool> DeleteExceptionFilter { get; set; } = _ => false;
	public Func<Exception, Task<IActionResult?>> HandleDeleteExceptionAsync { get; set; } = _ => Task.FromResult<IActionResult?>(null);
	public string CreatePolicyName { get; set; } = CorePolicyNames.Create;
	public string ReadPolicyName { get; set; } = CorePolicyNames.Read;
	public string UpdatePolicyName { get; set; } = CorePolicyNames.Update;
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