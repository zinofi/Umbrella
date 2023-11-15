using System.Globalization;
using Umbrella.DataAnnotations.BaseClasses;
using Umbrella.DataAnnotations.Utilities;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field is required contingent on whether another property
/// on the same object as the property this attribute is being used on matches conditions specified
/// using the constructor.
/// </summary>
/// <seealso cref="ContingentValidationAttribute" />
#pragma warning disable CA1813 // Avoid unsealed attributes
public class RequiredIfAttribute : ContingentValidationAttribute
#pragma warning restore CA1813 // Avoid unsealed attributes
{
	/// <summary>
	/// Gets the operator.
	/// </summary>
	public EqualityOperator Operator { get; }

	/// <summary>
	/// Gets the value that will be compared against the value of the <see cref="ContingentValidationAttribute.DependentProperty"/>
	/// </summary>
	public object ComparisonValue { get; }

	/// <summary>
	/// Gets the metadata.
	/// </summary>
	protected OperatorMetadata Metadata { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="operator">The operator.</param>
	/// <param name="comparisonValue">The comparison value.</param>
	public RequiredIfAttribute(string dependentProperty, EqualityOperator @operator, object comparisonValue)
		: base(dependentProperty)
	{
		Operator = @operator;
		ComparisonValue = comparisonValue;
		Metadata = OperatorMetadata.Get(Operator);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class with the <see cref="Operator"/> set to <see cref="EqualityOperator.EqualTo"/>.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="comparisonValue">The comparison value.</param>
	public RequiredIfAttribute(string dependentProperty, object comparisonValue)
		: this(dependentProperty, EqualityOperator.EqualTo, comparisonValue)
	{
	}

	/// <inheritdoc />
	public override string FormatErrorMessage(string name)
	{
		if (string.IsNullOrEmpty(ErrorMessageResourceName) && string.IsNullOrEmpty(ErrorMessage))
			ErrorMessage = DefaultErrorMessageFormat;

		return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, DependentProperty, ComparisonValue);
	}

	/// <inheritdoc />
	public override string ClientTypeName => "RequiredIf";

	/// <inheritdoc />
	protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters()
		=> base.GetClientValidationParameters()
			.Union(new[]
			{
				new KeyValuePair<string, object>("Operator", Operator.ToString()),
				new KeyValuePair<string, object>("DependentValue", ComparisonValue)
			});

	/// <inheritdoc />
	public override bool IsValid(object? value, object? actualDependentPropertyValue, object model)
		=> !Metadata.IsValid(actualDependentPropertyValue, ComparisonValue, ReturnTrueOnEitherNull) || (value is not null && !string.IsNullOrEmpty(value?.ToString()?.Trim()));

	/// <inheritdoc />
	public override string DefaultErrorMessageFormat => "{0} is required due to {1} being " + Metadata.ErrorMessage + " {2}";
}