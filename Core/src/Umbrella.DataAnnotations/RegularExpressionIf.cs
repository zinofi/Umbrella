using System.Collections.Generic;
using System.Linq;
using Umbrella.DataAnnotations.Utilities;

namespace Umbrella.DataAnnotations
{
	public class RegularExpressionIfAttribute : RequiredIfAttribute
	{
		/// <summary>
		/// Gets or sets the regex pattern.
		/// </summary>
		public string Pattern { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RegularExpressionIfAttribute"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="dependentProperty">The dependent property.</param>
		/// <param name="operator">The operator.</param>
		/// <param name="dependentValue">The dependent value.</param>
		public RegularExpressionIfAttribute(string pattern, string dependentProperty, Operator @operator, object dependentValue)
			: base(dependentProperty, @operator, dependentValue)
		{
			Pattern = pattern;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RegularExpressionIfAttribute"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="dependentProperty">The dependent property.</param>
		/// <param name="dependentValue">The dependent value.</param>
		public RegularExpressionIfAttribute(string pattern, string dependentProperty, object dependentValue)
			: this(pattern, dependentProperty, Operator.EqualTo, dependentValue)
		{
		}

		/// <inheritdoc />
		public override bool IsValid(object value, object dependentValue, object container)
			=> !Metadata.IsValid(dependentValue, DependentValue, ReturnTrueOnEitherNull) || OperatorMetadata.Get(Operator.RegExMatch).IsValid(value, Pattern, ReturnTrueOnEitherNull);

		/// <inheritdoc />
		protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters()
			=> base.GetClientValidationParameters()
				.Union(new[]
				{
					new KeyValuePair<string, object>("Pattern", Pattern),
				});

		/// <inheritdoc />
		public override string FormatErrorMessage(string name)
		{
			if (string.IsNullOrEmpty(ErrorMessageResourceName) && string.IsNullOrEmpty(ErrorMessage))
				ErrorMessage = DefaultErrorMessageFormat;

			return string.Format(ErrorMessageString, name, DependentProperty, DependentValue, Pattern);
		}

		/// <inheritdoc />
		public override string DefaultErrorMessageFormat => "{0} must be in the format of {3} due to {1} being " + Metadata.ErrorMessage + " {2}";
	}
}