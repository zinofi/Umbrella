using Umbrella.DataAnnotations.Utilities;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field is required to be a non-empty collection contingent on whether another property
/// on the same object as the property this attribute is being used on matches conditions specified
/// using the constructor.
/// </summary>
/// <seealso cref="RequiredIfAttribute" />
#pragma warning disable CA1813 // Avoid unsealed attributes
public class RequiredNonEmptyCollectionIfAttribute : RequiredIfAttribute
#pragma warning restore CA1813 // Avoid unsealed attributes
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredNonEmptyCollectionIfAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="operator">The operator.</param>
	/// <param name="comparisonValue">The dependent value.</param>
	public RequiredNonEmptyCollectionIfAttribute(string dependentProperty, EqualityOperator @operator, object comparisonValue)
		: base(dependentProperty, @operator, comparisonValue)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredNonEmptyCollectionIfAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="comparisonValue">The dependent value.</param>
	public RequiredNonEmptyCollectionIfAttribute(string dependentProperty, object comparisonValue)
		: this(dependentProperty, EqualityOperator.EqualTo, comparisonValue)
	{
	}

	/// <inheritdoc />
	public override bool IsValid(object? value, object? actualDependentPropertyValue, object model)
		=> !Metadata.IsValid(actualDependentPropertyValue, ComparisonValue, ReturnTrueOnEitherNull) || ValidationHelper.IsNonEmptyCollection(value);
}