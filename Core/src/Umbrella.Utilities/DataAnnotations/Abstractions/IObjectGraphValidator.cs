using System.ComponentModel.DataAnnotations;

namespace Umbrella.Utilities.DataAnnotations.Abstractions;

/// <summary>
/// A validator used to recursively validate an object graph which uses <see cref="ValidationAttribute"/>s.
/// </summary>
public interface IObjectGraphValidator
{
	/// <summary>
	/// Tries to validate object.
	/// </summary>
	/// <param name="instance">The instance.</param>
	/// <param name="validationContext">The validation context.</param>
	/// <param name="validateAllProperties">
	/// If <see langword="true"/> will validate all types of attribute.
	/// Otherwise it will only validate <see cref="RequiredAttribute"/>s. Defaults to <see langword="false"/> to keep things consistent with the <see cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult}, bool)" /> method.
	/// </param>
	/// <returns>A tuple containing value indicating if the object graph is valid together with a flatted collection of <see cref="ValidationResult"/> instances representing any errors.</returns>
	(bool isValid, IReadOnlyCollection<ObjectGraphValidationResult> results) TryValidateObject(object instance, ValidationContext? validationContext = null, bool validateAllProperties = false);
}