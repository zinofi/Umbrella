using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.DataAnnotations.Abstractions;
using Umbrella.Utilities.DataAnnotations.Options;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.DataAnnotations
{
	/// <summary>
	/// A validator used to recursively validate an object graph which uses <see cref="ValidationAttribute"/>s.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.DataAnnotations.Abstractions.IObjectGraphValidator" />
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
		/// Initializes a new instance of the <see cref="ObjectGraphValidator"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="options">The options.</param>
		public ObjectGraphValidator(
			ILogger<ObjectGraphValidator> logger,
			ObjectGraphValidatorOptions options)
		{
			Logger = logger;
			Options = options;
		}

		/// <inheritdoc />
		public virtual (bool isValid, IReadOnlyCollection<ValidationResult> results) TryValidateObject(object instance, ValidationContext? validationContext = null, bool validateAllProperties = false)
		{
			Guard.ArgumentNotNull(instance, nameof(instance));
			try
			{

				var lstVisited = new HashSet<object>();
				var lstValidationResult = new List<ValidationResult>();

				void ValidateObject(object value, ValidationContext? context = null)
				{
					if (value is null)
						return;

					if (!lstVisited.Add(value))
						return;

					// Check if we are dealing with a collection first as we will want to dig into it and validate each item recursively.
					if (value is IEnumerable enumerable)
					{
						foreach (object item in enumerable)
						{
							ValidateObject(item);
						}

						return;
					}

					// Validate the object
					Validator.TryValidateObject(value, context ?? new ValidationContext(value), lstValidationResult, validateAllProperties);

					// Now go through each public property on the object and apply validation
					foreach (PropertyInfo pi in value.GetType().GetProperties())
					{
						// Skip any properties we shouldn't be dealing with
						if (pi.PropertyType == typeof(string) || pi.PropertyType.IsPrimitive || (Options.IgnorePropertyFilter?.Invoke(pi.PropertyType) ?? false))
							continue;

						object child = pi.GetValue(value);
						ValidateObject(child);
					}
				}

				ValidateObject(instance, validationContext);

				return (lstValidationResult.Count is 0, lstValidationResult);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { validateAllProperties }, returnValue: true))
			{
				throw new UmbrellaException("An error has been encountered whilst validating the object graph.", exc);
			}
		}
	}
}