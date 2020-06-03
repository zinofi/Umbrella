using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.DataAnnotations
{
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
			if (!string.IsNullOrEmpty((dependentValue ?? string.Empty).ToString().Trim()))
				return value != null && !string.IsNullOrEmpty(value.ToString().Trim());

			return true;
		}

		/// <inheritdoc />
		public override string DefaultErrorMessageFormat => "{0} is required due to {1} not being empty.";
	}
}