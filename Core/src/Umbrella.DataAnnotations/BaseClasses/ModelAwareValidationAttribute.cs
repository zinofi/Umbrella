using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Umbrella.DataAnnotations.BaseClasses
{
	/// <summary>
	/// Serves as the base class for all model aware validation attributes.
	/// </summary>
	/// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
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
		/// <param name="value">The value.</param>
		/// <param name="container">The container.</param>
		/// <returns>
		///   <c>true</c> if the specified value is valid; otherwise, <c>false</c>.
		/// </returns>
		public abstract bool IsValid(object value, object container);

		/// <summary>
		/// Gets the name of the type for use in client scenarios, e.g. jQuery Validation, when used with web projects.
		/// </summary>
		public virtual string ClientTypeName => GetType().Name.Replace("Attribute", "");

		/// <summary>
		/// Gets the client validation parameters. Useful for web projects, e.g. jQuery Unobtrusive Validation, where these parameters
		/// are output by HTML Helpers (MVC 5) or Tag Helpers (ASP.NET Core) as data-* attributes.
		/// </summary>
		/// <returns>An enumerable key/value pair of the parameters.</returns>
		protected virtual IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters() => new KeyValuePair<string, object>[0];

		/// <summary>
		/// Gets the client validation parameters. Useful for web projects, e.g. jQuery Unobtrusive Validation, where these parameters
		/// are output by HTML Helpers (MVC 5) or Tag Helpers (ASP.NET Core) as data-* attributes.
		/// </summary>
		/// <returns>A Dictionary containing the parameters.</returns>
		public Dictionary<string, object> ClientValidationParameters => GetClientValidationParameters().ToDictionary(kv => kv.Key.ToLower(), kv => kv.Value);

		/// <inheritdoc />
		protected sealed override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			object container = validationContext.ObjectInstance;

			if (!IsValid(value, container))
				return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.MemberName is null ? null : new[] { validationContext.MemberName });

			return ValidationResult.Success;
		}
	}
}