namespace Umbrella.DataAnnotations.BaseClasses;

/// <summary>
/// Serves as the base class for all model aware validation attributes.
/// </summary>
/// <seealso cref="ValidationAttribute" />
[AttributeUsage(AttributeTargets.Property)]
public abstract class ModelAwareValidationAttribute : ValidationAttribute
{
	/// <inheritdoc />
	public override bool IsValid(object value) => throw new NotImplementedException();

	/// <inheritdoc />
	public override string FormatErrorMessage(string name)
	{
		if (string.IsNullOrEmpty(ErrorMessageResourceName) && string.IsNullOrEmpty(ErrorMessage))
			ErrorMessage = DefaultErrorMessageFormat;

		return base.FormatErrorMessage(name);
	}

	/// <summary>
	/// Gets the default error message format.
	/// </summary>
	public virtual string DefaultErrorMessageFormat => "{0} is invalid.";

	/// <summary>
	/// Returns true if the value is valid.
	/// </summary>
	/// <remarks>
	/// Although this method can be called manually, it is automatically called internally
	/// by the built-in <see cref="ValidationAttribute.IsValid(object, ValidationContext)"/> which has been overridden
	/// in the <see cref="ModelAwareValidationAttribute"/> base class.
	/// </remarks>
	/// <param name="value">The value.</param>
	/// <param name="model">The model.</param>
	/// <returns>
	///   <c>true</c> if the specified value is valid; otherwise, <c>false</c>.
	/// </returns>
	public abstract bool IsValid(object value, object model);

	/// <summary>
	/// Gets the name of the type for use in client scenarios, e.g. jQuery Validation, when used with web projects.
	/// </summary>
	public virtual string ClientTypeName => GetType().Name.Replace("Attribute", "");

	/// <summary>
	/// Gets the client validation parameters. Useful for web projects, e.g. jQuery Unobtrusive Validation, where these parameters
	/// are output by HTML Helpers (MVC 5) or Tag Helpers (ASP.NET Core) as data-* attributes.
	/// </summary>
	/// <returns>An enumerable key/value pair of the parameters.</returns>
	protected virtual IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters() => Array.Empty<KeyValuePair<string, object>>();

	/// <summary>
	/// Gets the client validation parameters. Useful for web projects, e.g. jQuery Unobtrusive Validation, where these parameters
	/// are output by HTML Helpers (MVC 5) or Tag Helpers (ASP.NET Core) as data-* attributes.
	/// </summary>
	/// <returns>A Dictionary containing the parameters.</returns>
	public Dictionary<string, object> ClientValidationParameters => GetClientValidationParameters().ToDictionary(kv => kv.Key.ToLowerInvariant(), kv => kv.Value);

	/// <inheritdoc />
	protected sealed override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		object model = validationContext.ObjectInstance;

		if (!IsValid(value, model))
			return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.MemberName is null ? null : new[] { validationContext.MemberName });

		return ValidationResult.Success;
	}
}