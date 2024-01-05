using System.ComponentModel.DataAnnotations;

namespace Umbrella.Utilities.DataAnnotations;

/// <summary>
/// The result of a validation request for an object graph.
/// </summary>
public class ObjectGraphValidationResult(ValidationResult validationResult, object model) : ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames)
{
	/// <summary>
	/// The model that was validated.
	/// </summary>
	public object Model { get; } = model;
}