using System.Diagnostics.CodeAnalysis;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Provides a base attribute for performing asynchronous validation on a property, field, or parameter using a custom
/// validation logic.
/// </summary>
/// <remarks>
/// <para>
/// Derive from this class to implement asynchronous validation logic for data annotations. Unlike
/// standard ValidationAttribute, this base class enables validation methods that return a Task, allowing for
/// non-blocking operations such as database or remote service checks. Override the IsValidAsync method to define the
/// asynchronous validation behavior. This attribute is intended for use in scenarios where validation may require
/// asynchronous operations.
/// </para>
/// <para>
/// Please note that this attribute will only function correctly in validation frameworks that support asynchronous
/// validation. In contexts that do not support async validation, the synchronous IsValid method will block.
/// </para>
/// </remarks>
public abstract class AsyncValidationAttribute : ValidationAttribute
{
	/// <summary>
	/// Asynchronously determines whether the specified value is valid with respect to the provided validation context.
	/// </summary>
	/// <param name="value">The value to validate. May be null if the validation logic allows it.</param>
	/// <param name="validationContext">The context information about the validation operation, including the object instance and additional metadata.
	/// Cannot be null.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous validation operation. The task result contains a ValidationResult that
	/// describes the validation outcome, or null if validation is successful.</returns>
	protected abstract Task<ValidationResult?> IsValidAsync(object? value, ValidationContext validationContext, CancellationToken cancellationToken);

	/// <inheritdoc />
	[SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "Necessary evil for pipelines that don't support async validation.")]
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) => IsValidAsync(value, validationContext, default).GetAwaiter().GetResult();

	/// <inheritdoc />
	public sealed override bool IsValid(object? value) => throw new NotSupportedException("Use the overload that accepts a ValidationContext.");

	/// <summary>
	///     Tests whether the given <paramref name="value" /> is valid with respect to the current
	///     validation attribute without throwing a <see cref="ValidationException" />
	/// </summary>
	/// <remarks>
	///     If this method returns <see cref="ValidationResult.Success" />, then validation was successful, otherwise
	///     an instance of <see cref="ValidationResult" /> will be returned with a guaranteed non-null
	///     <see cref="ValidationResult.ErrorMessage" />.
	/// </remarks>
	/// <param name="value">The value to validate</param>
	/// <param name="validationContext">
	///     A <see cref="ValidationContext" /> instance that provides
	///     context about the validation operation, such as the object and member being validated.
	/// </param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>
	///     When validation is valid, <see cref="ValidationResult.Success" />.
	///     <para>
	///         When validation is invalid, an instance of <see cref="ValidationResult" />.
	///     </para>
	/// </returns>
	/// <exception cref="InvalidOperationException"> is thrown if the current attribute is malformed.</exception>
	/// <exception cref="ArgumentNullException">When <paramref name="validationContext" /> is null.</exception>
	public async Task<ValidationResult?> GetValidationResultAsync(object? value, ValidationContext validationContext, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (validationContext is null)
			throw new ArgumentNullException(nameof(validationContext));

		ValidationResult? result = await IsValidAsync(value, validationContext, cancellationToken);

		// If validation fails, we want to ensure we have a ValidationResult that guarantees it has an ErrorMessage
		if (result is not null)
		{
			if (string.IsNullOrEmpty(result.ErrorMessage))
			{
				string errorMessage = FormatErrorMessage(validationContext.DisplayName);
				result = new ValidationResult(errorMessage, result.MemberNames);
			}
		}

		return result;
	}
}