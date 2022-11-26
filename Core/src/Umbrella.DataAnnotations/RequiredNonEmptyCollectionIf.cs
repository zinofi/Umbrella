using Umbrella.DataAnnotations.Utilities;

namespace Umbrella.DataAnnotations;

public class RequiredNonEmptyCollectionIfAttribute : RequiredIfAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredNonEmptyCollectionIfAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="operator">The operator.</param>
	/// <param name="dependentValue">The dependent value.</param>
	public RequiredNonEmptyCollectionIfAttribute(string dependentProperty, Operator @operator, object dependentValue)
		: base(dependentProperty, @operator, dependentValue)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredNonEmptyCollectionIfAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	/// <param name="dependentValue">The dependent value.</param>
	public RequiredNonEmptyCollectionIfAttribute(string dependentProperty, object dependentValue)
		: this(dependentProperty, Operator.EqualTo, dependentValue)
	{
	}

	/// <inheritdoc />
	public override bool IsValid(object value, object dependentValue, object container)
		=> !Metadata.IsValid(dependentValue, DependentValue, ReturnTrueOnEitherNull) || ValidationHelper.IsNonEmptyCollection(value);
}