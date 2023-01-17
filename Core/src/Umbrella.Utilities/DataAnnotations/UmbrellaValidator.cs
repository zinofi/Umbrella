// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.DataAnnotations.Abstractions;

/* Unmerged change from project 'Umbrella.Utilities(net461)'
Before:
namespace Umbrella.Utilities.DataAnnotations
{
	/// <summary>
	/// A validation utility used to perform validation on an object instance which uses <see cref="ValidationAttribute"/>s.
	/// </summary>
	public class UmbrellaValidator : IUmbrellaValidator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaValidator"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="objectGraphValidator">The object graph validator.</param>
		public UmbrellaValidator(
			ILogger<UmbrellaValidator> logger,
			IObjectGraphValidator objectGraphValidator)
		{
			Logger = logger;
			ObjectGraphValidator = objectGraphValidator;
		}

		/// <summary>
		/// Gets the logger.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the graph validator.
		/// </summary>
		protected IObjectGraphValidator ObjectGraphValidator { get; }

		/// <inheritdoc />
		public (bool isValid, IReadOnlyCollection<ValidationResult> results) ValidateItemAsync(object item, ValidationType validationType)
		{
			if (validationType == ValidationType.None)
				return (true, Array.Empty<ValidationResult>());

			var ctx = new ValidationContext(item);

			if (validationType == ValidationType.Deep)
				return ObjectGraphValidator.TryValidateObject(item, ctx, true);

			if (validationType == ValidationType.Shallow)
			{
				var lstResult = new List<ValidationResult>();

				bool valid = Validator.TryValidateObject(item, ctx, lstResult, true);

				return (valid, lstResult);
			}

			throw new UmbrellaException("Unknown validation type.");
		}
After:
namespace Umbrella.Utilities.DataAnnotations;

/// <summary>
/// A validation utility used to perform validation on an object instance which uses <see cref="ValidationAttribute"/>s.
/// </summary>
public class UmbrellaValidator : IUmbrellaValidator
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaValidator"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="objectGraphValidator">The object graph validator.</param>
	public UmbrellaValidator(
		ILogger<UmbrellaValidator> logger,
		IObjectGraphValidator objectGraphValidator)
	{
		Logger = logger;
		ObjectGraphValidator = objectGraphValidator;
	}

	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets the graph validator.
	/// </summary>
	protected IObjectGraphValidator ObjectGraphValidator { get; }

	/// <inheritdoc />
	public (bool isValid, IReadOnlyCollection<ValidationResult> results) ValidateItemAsync(object item, ValidationType validationType)
	{
		if (validationType == ValidationType.None)
			return (true, Array.Empty<ValidationResult>());

		var ctx = new ValidationContext(item);

		if (validationType == ValidationType.Deep)
			return ObjectGraphValidator.TryValidateObject(item, ctx, true);

		if (validationType == ValidationType.Shallow)
		{
			var lstResult = new List<ValidationResult>();

			bool valid = Validator.TryValidateObject(item, ctx, lstResult, true);

			return (valid, lstResult);
		}

		throw new UmbrellaException("Unknown validation type.");
*/
using Umbrella.Utilities.DataAnnotations.Enumerations;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.DataAnnotations;

/// <summary>
/// A validation utility used to perform validation on an object instance which uses <see cref="ValidationAttribute"/>s.
/// </summary>
public class UmbrellaValidator : IUmbrellaValidator
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaValidator"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="objectGraphValidator">The object graph validator.</param>
	public UmbrellaValidator(
		ILogger<UmbrellaValidator> logger,
		IObjectGraphValidator objectGraphValidator)
	{
		Logger = logger;
		ObjectGraphValidator = objectGraphValidator;
	}

	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets the graph validator.
	/// </summary>
	protected IObjectGraphValidator ObjectGraphValidator { get; }

	/// <inheritdoc />
	public (bool isValid, IReadOnlyCollection<ValidationResult> results) ValidateItem(object item, ValidationType validationType)
	{
		if (validationType is ValidationType.None)
			return (true, Array.Empty<ValidationResult>());

		var ctx = new ValidationContext(item);

		if (validationType is ValidationType.Deep)
			return ObjectGraphValidator.TryValidateObject(item, ctx, true);

		if (validationType is ValidationType.Shallow)
		{
			var lstResult = new List<ValidationResult>();

			bool valid = Validator.TryValidateObject(item, ctx, lstResult, true);

			return (valid, lstResult);
		}

		throw new UmbrellaException("Unknown validation type.");
	}
}