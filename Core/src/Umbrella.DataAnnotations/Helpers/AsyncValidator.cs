using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Umbrella.DataAnnotations.Helpers;

public static class AsyncValidator
{
	private static readonly ValidationAttributeStore _store = ValidationAttributeStore.Instance;

	public static async Task<bool> TryValidatePropertyAsync(object? value, ValidationContext validationContext,
		ICollection<ValidationResult>? validationResults, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		
		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));

		var propertyType = _store.GetPropertyType(validationContext);
		string propertyName = validationContext.MemberName!;
		
		EnsureValidPropertyType(propertyName, propertyType, value);

		bool result = true;
		bool breakOnFirstError = validationResults == null;
		
		var attributes = _store.GetPropertyValidationAttributes(validationContext);
		
		var errors = await GetValidationErrorsAsync(value, validationContext, attributes, breakOnFirstError, cancellationToken).ConfigureAwait(false);
		
		foreach (var err in errors)
		{
			result = false;
			validationResults?.Add(err.ValidationResult);
		}

		return result;
	}

	public static Task<bool> TryValidateObjectAsync(object instance, ValidationContext validationContext, ICollection<ValidationResult>? validationResults, CancellationToken cancellationToken = default)
		=> TryValidateObjectAsync(instance, validationContext, validationResults, false, cancellationToken);

	public static async Task<bool> TryValidateObjectAsync(object instance, ValidationContext validationContext,
		ICollection<ValidationResult>? validationResults, bool validateAllProperties, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (instance is null)
			throw new ArgumentNullException(nameof(instance));
		
		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));
		
		if (instance != validationContext.ObjectInstance)
			throw new ArgumentException(SR.Validator_InstanceMustMatchValidationContextInstance, nameof(instance));

		bool result = true;
		bool breakOnFirstError = validationResults == null;
		
		var errors = await GetObjectValidationErrorsAsync(instance, validationContext, validateAllProperties, breakOnFirstError, cancellationToken).ConfigureAwait(false);
		
		foreach (var err in errors)
		{
			result = false;
			validationResults?.Add(err.ValidationResult);
		}

		return result;
	}

	public static async Task<bool> TryValidateValueAsync(object? value, ValidationContext validationContext,
		ICollection<ValidationResult>? validationResults, IEnumerable<ValidationAttribute> validationAttributes, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));

		if (validationAttributes is null)
			throw new ArgumentNullException(nameof(validationAttributes));
		
		bool result = true;
		bool breakOnFirstError = validationResults is null;
		
		var errors = await GetValidationErrorsAsync(value, validationContext, validationAttributes, breakOnFirstError, cancellationToken).ConfigureAwait(false);
		
		foreach (var err in errors)
		{
			result = false;
			validationResults?.Add(err.ValidationResult);
		}

		return result;
	}

	public static async Task ValidatePropertyAsync(object? value, ValidationContext validationContext, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		
		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));
		
		var propertyType = _store.GetPropertyType(validationContext);
		
		EnsureValidPropertyType(validationContext.MemberName!, propertyType, value);
		
		var attributes = _store.GetPropertyValidationAttributes(validationContext);
		
		var errors = await GetValidationErrorsAsync(value, validationContext, attributes, false, cancellationToken).ConfigureAwait(false);
		
		if (errors.Count > 0)
			errors[0].ThrowValidationException();
	}

	public static Task ValidateObjectAsync(object instance, ValidationContext validationContext, CancellationToken cancellationToken = default)
		=> ValidateObjectAsync(instance, validationContext, false, cancellationToken);

	public static async Task ValidateObjectAsync(object instance, ValidationContext validationContext,
		bool validateAllProperties, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (instance is null)
			throw new ArgumentNullException(nameof(instance));
		
		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));
		
		if (instance != validationContext.ObjectInstance)
			throw new ArgumentException(SR.Validator_InstanceMustMatchValidationContextInstance, nameof(instance));
		
		var errors = await GetObjectValidationErrorsAsync(instance, validationContext, validateAllProperties, false, cancellationToken).ConfigureAwait(false);
		
		if (errors.Count > 0)
			errors[0].ThrowValidationException();
	}

	public static async Task ValidateValueAsync(object? value, ValidationContext validationContext,
		IEnumerable<ValidationAttribute> validationAttributes, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));

		if (validationAttributes is null)
			throw new ArgumentNullException(nameof(validationAttributes));
		
		var errors = await GetValidationErrorsAsync(value, validationContext, validationAttributes, false, cancellationToken).ConfigureAwait(false);
		
		if (errors.Count > 0)
			errors[0].ThrowValidationException();
	}

	private static async Task<List<ValidationError>> GetObjectValidationErrorsAsync(object instance,
		ValidationContext validationContext, bool validateAllProperties, bool breakOnFirstError, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		
		var errors = await GetObjectPropertyValidationErrorsAsync(instance, validationContext, validateAllProperties, breakOnFirstError, cancellationToken).ConfigureAwait(false);
		
		if (errors.Count > 0)
			return errors;

		var attributes = _store.GetTypeValidationAttributes(validationContext);
		
		errors.AddRange(await GetValidationErrorsAsync(instance, validationContext, attributes, breakOnFirstError, cancellationToken).ConfigureAwait(false));
		
		if (errors.Count > 0)
			return errors;

		if (instance is IValidatableObject validatable)
		{
			var results = validatable.Validate(validationContext);
			
			if (results != null)
			{
				foreach (var result in results)
				{
					if (result != ValidationResult.Success)
					{
						errors.Add(new ValidationError(null, instance, result));
					}
				}
			}
		}

		return errors;
	}

	private static async Task<List<ValidationError>> GetObjectPropertyValidationErrorsAsync(object instance,
		ValidationContext validationContext, bool validateAllProperties, bool breakOnFirstError, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		
		var properties = GetPropertyValues(instance, validationContext);
		var errors = new List<ValidationError>();
		
		foreach (var property in properties)
		{
			var attributes = _store.GetPropertyValidationAttributes(property.Key);
			
			if (validateAllProperties)
			{
				errors.AddRange(await GetValidationErrorsAsync(property.Value, property.Key, attributes, breakOnFirstError, cancellationToken).ConfigureAwait(false));
			}
			else
			{
				foreach (ValidationAttribute attribute in attributes)
				{
					if (attribute is RequiredAttribute reqAttr)
					{
						ValidationResult? validationResult = attribute is AsyncValidationAttribute asyncAttr
							? await asyncAttr.GetValidationResultAsync(property.Value, property.Key, cancellationToken).ConfigureAwait(false)
							: reqAttr.GetValidationResult(property.Value, property.Key);
						
						if (validationResult != ValidationResult.Success)
							errors.Add(new ValidationError(reqAttr, property.Value, validationResult!));
						
						break;
					}
				}
			}

			if (breakOnFirstError && errors.Count > 0)
				break;
		}

		return errors;
	}

	private static async Task<List<ValidationError>> GetValidationErrorsAsync(object? value,
		ValidationContext validationContext, IEnumerable<ValidationAttribute> attributes, bool breakOnFirstError, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		
		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));
		
		var errors = new List<ValidationError>();
		
		ValidationError? validationError;
		RequiredAttribute? required = null;
		
		foreach (var attribute in attributes)
		{
			required = attribute as RequiredAttribute;

			if (required is not null)
			{
				if (!await TryValidateAsync(value, validationContext, required, cancellationToken).ConfigureAwait(false))
				{
					validationError = new ValidationError(required, value, required.GetValidationResult(value, validationContext)!);
					
					errors.Add(validationError);
					
					return errors; // Required failure aborts
				}

				break;
			}
		}

		foreach (var attr in attributes)
		{
			if (attr != required)
			{
				if (!await TryValidateAsync(value, validationContext, attr, cancellationToken).ConfigureAwait(false))
				{
					var validationResult = attr is AsyncValidationAttribute asyncAttr
						? await asyncAttr.GetValidationResultAsync(value, validationContext, cancellationToken).ConfigureAwait(false)
						: attr.GetValidationResult(value, validationContext);
					
					validationError = new ValidationError(attr, value, validationResult!);
					
					errors.Add(validationError);
					
					if (breakOnFirstError)
						break;
				}
			}
		}

		return errors;
	}

	private static async Task<bool> TryValidateAsync(object? value, ValidationContext validationContext, ValidationAttribute attribute, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Debug.Assert(validationContext != null);
		
		ValidationResult? validationResult = attribute is AsyncValidationAttribute asyncAttribute
			? await asyncAttribute.GetValidationResultAsync(value, validationContext!, cancellationToken).ConfigureAwait(false)
			: attribute.GetValidationResult(value, validationContext!);
		
		return validationResult == ValidationResult.Success;
	}

	private static ValidationContext CreateValidationContext(object instance, ValidationContext validationContext)
	{
		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));
		return new ValidationContext(instance, validationContext, validationContext.Items);
	}

	private static bool CanBeAssigned(Type destinationType, object? value)
	{
		if (value is null)
			return !destinationType.IsValueType || (destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>));
		
		return destinationType.IsInstanceOfType(value);
	}

	private static void EnsureValidPropertyType(string propertyName, Type propertyType, object? value)
	{
		if (!CanBeAssigned(propertyType, value))
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.Validator_Property_Value_Wrong_Type, propertyName, propertyType), nameof(value));
	}

	private static List<KeyValuePair<ValidationContext, object?>> GetPropertyValues(object instance, ValidationContext validationContext)
	{
		var properties = TypeDescriptor.GetProperties(instance.GetType());
		var items = new List<KeyValuePair<ValidationContext, object?>>(properties.Count);
		
		foreach (PropertyDescriptor property in properties)
		{
			var context = CreateValidationContext(instance, validationContext);
			context.MemberName = property.Name;
			
			if (_store.GetPropertyValidationAttributes(context).Any())
				items.Add(new KeyValuePair<ValidationContext, object?>(context, property.GetValue(instance)));
		}

		return items;
	}

	private sealed class ValidationError
	{
		private readonly object? _value;
		private readonly ValidationAttribute? _validationAttribute;
		
		internal ValidationError(ValidationAttribute? validationAttribute, object? value, ValidationResult validationResult)
		{
			_validationAttribute = validationAttribute;
			ValidationResult = validationResult;
			_value = value;
		}

		internal ValidationResult ValidationResult { get; }

		[DoesNotReturn]
		internal void ThrowValidationException() => throw new ValidationException(ValidationResult, _validationAttribute, _value);
	}
}