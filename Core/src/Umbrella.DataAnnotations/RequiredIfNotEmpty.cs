using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// Specifies that a data field is required contingent on whether another string property on the same
	/// object is not null, an empty string or only whitespace.
	/// </summary>
	/// <seealso cref="ContingentValidationAttribute" />
	public class RequiredIfNotEmptyAttribute : ContingentValidationAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredIfNotEmptyAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public RequiredIfNotEmptyAttribute(string dependentProperty)
			: base(dependentProperty)
		{
		}

		/// <inheritdoc />
		public override bool IsValid(object value, object dependentValue, object container)
		{
			if (!string.IsNullOrWhiteSpace((dependentValue ?? string.Empty).ToString()))
				return value is not null && !string.IsNullOrWhiteSpace(value.ToString());

			return true;
		}

		/// <inheritdoc />
		public override string DefaultErrorMessageFormat => "{0} is required due to {1} not being empty.";
	}
}