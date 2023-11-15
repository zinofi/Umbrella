﻿namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that the value of a property should be less than the value of another named property on the same type.
/// </summary>
/// <seealso cref="IsAttribute" />
public sealed class LessThanAttribute : IsAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LessThanAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	public LessThanAttribute(string dependentProperty)
		: base(EqualityOperator.LessThan, dependentProperty)
	{
	}
}