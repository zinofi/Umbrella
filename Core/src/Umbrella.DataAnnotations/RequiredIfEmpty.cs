using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.DataAnnotations
{
	public class RequiredIfEmptyAttribute : ContingentValidationAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredIfEmptyAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public RequiredIfEmptyAttribute(string dependentProperty)
			: base(dependentProperty) { }

		/// <inheritdoc />
		public override bool IsValid(object value, object dependentValue, object container)
			=> !string.IsNullOrEmpty((dependentValue ?? string.Empty).ToString().Trim()) || value != null && !string.IsNullOrEmpty(value.ToString().Trim());

		/// <inheritdoc />
		public override string DefaultErrorMessageFormat => "{0} is required due to {1} being empty.";
	}
}