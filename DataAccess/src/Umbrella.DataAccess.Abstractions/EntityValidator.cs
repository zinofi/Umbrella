using System;
using System.ComponentModel.DataAnnotations;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// A collection of helper methods used for entity validation.
	/// </summary>
	/// <seealso cref="IEntityValidator" />
	public class EntityValidator : IEntityValidator
	{
		/// <summary>
		/// Validates the length of the property string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="minLength">The minimum length.</param>
		/// <param name="maxLength">The maximum length.</param>
		/// <param name="required">Specifies if a value is required for the property.</param>
		/// <returns>
		/// The validation result.
		/// </returns>
		public ValidationResult ValidatePropertyStringLength(string value, string propertyName, int minLength, int maxLength, bool required = true)
			=> !value.IsValidLength(minLength, maxLength, !required)
				? new ValidationResult(string.Format(ErrorMessages.InvalidPropertyStringLengthErrorMessageFormat, propertyName, minLength, maxLength), new[] { propertyName })
				: ValidationResult.Success;

		/// <summary>
		/// Validates the property number range.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="min">The minimum.</param>
		/// <param name="max">The maximum.</param>
		/// <param name="required">Specifies if a value is required for the property.</param>
		/// <returns>
		/// The validation result.
		/// </returns>
		public ValidationResult ValidatePropertyNumberRange<TProperty>(TProperty? value, string propertyName, TProperty min, TProperty max, bool required = true)
			where TProperty : struct, IComparable<TProperty>
			=> !value.IsValidRange(min, max, !required)
				? new ValidationResult(string.Format(ErrorMessages.InvalidPropertyNumberRangeErrorMessageFormat, propertyName, min, max), new[] { propertyName })
				: ValidationResult.Success;
	}
}