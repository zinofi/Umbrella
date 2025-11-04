// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.DataAnnotations.Helpers; // Reference AsyncValidator
using Umbrella.Utilities.DataAnnotations.Abstractions;
using Umbrella.Utilities.DataAnnotations.Options;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.DataAnnotations;

/// <summary>
/// A validator used to recursively validate an object graph which uses <see cref="ValidationAttribute"/>s.
/// </summary>
/// <seealso cref="IObjectGraphValidator" />
public class ObjectGraphValidator : IObjectGraphValidator
{
	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets or sets the options.
	/// </summary>
	protected ObjectGraphValidatorOptions Options { get; set; }

	/// <summary>
	/// Gets the service provider used to resolve application services.
	/// </summary>
	protected IServiceProvider ServiceProvider { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ObjectGraphValidator"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="options">The options.</param>
	/// <param name="serviceProvider">The service provider.</param>
	public ObjectGraphValidator(
		ILogger<ObjectGraphValidator> logger,
		ObjectGraphValidatorOptions options,
		IServiceProvider serviceProvider)
	{
		Logger = logger;
		Options = options;
		ServiceProvider = serviceProvider;
	}

	/// <inheritdoc />
	public (bool isValid, IReadOnlyCollection<ObjectGraphValidationResult> results) TryValidateObject(object instance, ValidationContext? validationContext = null, bool validateAllProperties = false)
	{
		Guard.IsNotNull(instance, nameof(instance));

		try
		{
			var lstVisited = new HashSet<object>();
			var lstValidationResult = new List<ObjectGraphValidationResult>();

			void ValidateObject(object value, ValidationContext? context = null)
			{
				if (value is null)
				{
					return;
				}

				if (!lstVisited.Add(value))
				{
					return;
				}

				// Check if we are dealing with a collection first as we will want to dig into it and validate each item recursively.
				if (value is IEnumerable enumerable and not string)
				{
					foreach (object item in enumerable)
					{
						// Skip any properties we shouldn't be dealing with
						var type = item.GetType();
						
						if (item is string s || type.IsPrimitive || (Options.IgnorePropertyFilter?.Invoke(type) ?? false))
						{
							continue;
						}

						ValidateObject(item);
					}

					return;
				}

				// Validate the object
				List<ValidationResult> lstInnerValidationResult = [];

				_ = Validator.TryValidateObject(value, context ?? new ValidationContext(value, ServiceProvider, null), lstInnerValidationResult, validateAllProperties);

				lstValidationResult.AddRange(lstInnerValidationResult.Select(x => new ObjectGraphValidationResult(x, value)));

				// Now go through each public property on the object and apply validation
				foreach (PropertyInfo pi in value.GetType().GetProperties())
				{
					// Skip any properties we shouldn't be dealing with
					if (pi.PropertyType == typeof(string) || pi.PropertyType.IsPrimitive || (Options.IgnorePropertyFilter?.Invoke(pi.PropertyType) ?? false))
					{
						continue;
					}

					object? child = pi.GetValue(value);

					if (child is not null)
						ValidateObject(child);
				}
			}

			ValidateObject(instance, validationContext);

			return (lstValidationResult.Count is 0, lstValidationResult);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { validateAllProperties }))
		{
			throw new UmbrellaException("An error has been encountered whilst validating the object graph.", exc);
		}
	}

	/// <inheritdoc />
	public async Task<(bool isValid, IReadOnlyCollection<ObjectGraphValidationResult> results)> TryValidateObjectAsync(object instance, ValidationContext? validationContext = null, bool validateAllProperties = false, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(instance, nameof(instance));

		try
		{
			var lstVisited = new HashSet<object>();
			var lstValidationResult = new List<ObjectGraphValidationResult>();

			async Task ValidateObjectAsync(object value, ValidationContext? context = null)
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (value is null)
				{
					return;
				}

				if (!lstVisited.Add(value))
				{
					return;
				}

				if (value is IEnumerable enumerable and not string)
				{
					foreach (object item in enumerable)
					{
						// Skip any properties we shouldn't be dealing with
						var type = item.GetType();

						if (item is string s || type.IsPrimitive || (Options.IgnorePropertyFilter?.Invoke(type) ?? false))
						{
							continue;
						}

						await ValidateObjectAsync(item).ConfigureAwait(false);
					}

					return;
				}

				List<ValidationResult> lstInnerValidationResult = [];

				var ctx = context ?? new ValidationContext(value, ServiceProvider, null);

				_ = await AsyncValidator.TryValidateObjectAsync(value, ctx, lstInnerValidationResult, validateAllProperties, cancellationToken).ConfigureAwait(false);
				
				lstValidationResult.AddRange(lstInnerValidationResult.Select(x => new ObjectGraphValidationResult(x, value)));
				
				foreach (PropertyInfo pi in value.GetType().GetProperties())
				{
					if (pi.PropertyType == typeof(string) || pi.PropertyType.IsPrimitive || (Options.IgnorePropertyFilter?.Invoke(pi.PropertyType) ?? false))
					{
						continue;
					}

					object? child = pi.GetValue(value);
					
					if (child is not null)
					{
						await ValidateObjectAsync(child).ConfigureAwait(false);
					}
				}
			}

			await ValidateObjectAsync(instance, validationContext).ConfigureAwait(false);
			
			return (lstValidationResult.Count is 0, lstValidationResult);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { validateAllProperties }))
		{
			throw new UmbrellaException("An error has been encountered whilst validating the object graph asynchronously.", exc);
		}
	}
}