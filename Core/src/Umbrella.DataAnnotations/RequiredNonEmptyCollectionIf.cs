using Umbrella.DataAnnotations.Utilities;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field is required to be a non-empty collection contingent on whether another property
/// on the same object as the property this attribute is being used on matches conditions specified
/// using the constructor.
/// </summary>
/// <seealso cref="RequiredIfAttribute" />
public class RequiredNonEmptyCollectionIfAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredNonEmptyCollectionIfAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="operator">The operator.</param>
	/// <param name="dependentValue">The dependent value.</param>
	public RequiredNonEmptyCollectionIfAttribute(string dependentProperty, EqualityOperator @operator, object dependentValue)
		: base(dependentProperty, @operator, dependentValue)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredNonEmptyCollectionIfAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="dependentValue">The dependent value.</param>
	public RequiredNonEmptyCollectionIfAttribute(string dependentProperty, object dependentValue)
		: this(dependentProperty, EqualityOperator.EqualTo, dependentValue)
	{
	}

	/// <inheritdoc />
	public override bool IsValid(object value, object dependentValue, object container)
		=> !Metadata.IsValid(dependentValue, DependentValue, ReturnTrueOnEitherNull) || ValidationHelper.IsNonEmptyCollection(value);
}