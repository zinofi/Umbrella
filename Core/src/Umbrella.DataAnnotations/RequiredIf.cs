using System.Collections.Generic;
using System.Linq;
using Umbrella.DataAnnotations.BaseClasses;
using Umbrella.DataAnnotations.Utilities;

namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// Specifies that a data field is required contingent on whether another property
	/// on the same object as the property this attribute is being used on matches conditions specified
	/// using the constructor.
	/// </summary>
	/// <seealso cref="Umbrella.DataAnnotations.BaseClasses.ContingentValidationAttribute" />
	public class RequiredIfAttribute : ContingentValidationAttribute
	{
		/// <summary>
		/// Gets the operator.
		/// </summary>
		public Operator Operator { get; }

		/// <summary>
		/// Gets the dependent value.
		/// </summary>
		public object DependentValue { get; }

		/// <summary>
		/// Gets the metadata.
		/// </summary>
		protected OperatorMetadata Metadata { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		/// <param name="operator">The operator.</param>
		/// <param name="dependentValue">The dependent value.</param>
		public RequiredIfAttribute(string dependentProperty, Operator @operator, object dependentValue)
			: base(dependentProperty)
		{
			Operator = @operator;
			DependentValue = dependentValue;
			Metadata = OperatorMetadata.Get(Operator);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class with the <see cref="Operator"/> set to <see cref="Operator.EqualTo"/>.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		/// <param name="dependentValue">The dependent value.</param>
		public RequiredIfAttribute(string dependentProperty, object dependentValue)
			: this(dependentProperty, Operator.EqualTo, dependentValue)
		{
		}

		/// <inheritdoc />
		public override string FormatErrorMessage(string name)
		{
			if (string.IsNullOrEmpty(ErrorMessageResourceName) && string.IsNullOrEmpty(ErrorMessage))
				ErrorMessage = DefaultErrorMessageFormat;

			return string.Format(ErrorMessageString, name, DependentProperty, DependentValue);
		}

		/// <inheritdoc />
		public override string ClientTypeName => "RequiredIf";

		/// <inheritdoc />
		protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters()
			=> base.GetClientValidationParameters()
				.Union(new[]
				{
					new KeyValuePair<string, object>("Operator", Operator.ToString()),
					new KeyValuePair<string, object>("DependentValue", DependentValue)
				});

		/// <inheritdoc />
		public override bool IsValid(object value, object dependentValue, object container)
			=> !Metadata.IsValid(dependentValue, DependentValue, ReturnTrueOnEitherNull) || value is not null && !string.IsNullOrEmpty(value.ToString().Trim());

		/// <inheritdoc />
		public override string DefaultErrorMessageFormat => "{0} is required due to {1} being " + Metadata.ErrorMessage + " {2}";
	}
}