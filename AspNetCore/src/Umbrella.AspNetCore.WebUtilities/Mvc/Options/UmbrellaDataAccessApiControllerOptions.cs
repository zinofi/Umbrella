﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.Options
{
	public class UmbrellaDataAccessApiControllerOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
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
		public string CreatePolicyName { get; set; } = null!;
		public string ReadPolicyName { get; set; } = null!;
		public string UpdatePolicyName { get; set; } = null!;
		public string DeletePolicyName { get; set; } = null!;

		/// <inheritdoc />
		public void Sanitize()
		{
			CreatePolicyName = CreatePolicyName?.Trim()!;
			ReadPolicyName = ReadPolicyName?.Trim()!;
			UpdatePolicyName = UpdatePolicyName?.Trim()!;
			DeletePolicyName = DeletePolicyName?.Trim()!;
		}

		/// <inheritdoc />
		public void Validate()
		{
			Guard.ArgumentNotNullOrEmpty(CreatePolicyName, nameof(CreatePolicyName));
			Guard.ArgumentNotNullOrEmpty(ReadPolicyName, nameof(ReadPolicyName));
			Guard.ArgumentNotNullOrEmpty(UpdatePolicyName, nameof(UpdatePolicyName));
			Guard.ArgumentNotNullOrEmpty(DeletePolicyName, nameof(DeletePolicyName));
		}
	}
}