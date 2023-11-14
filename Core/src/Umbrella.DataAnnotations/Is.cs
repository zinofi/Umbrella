using Umbrella.DataAnnotations.BaseClasses;
using Umbrella.DataAnnotations.Utilities;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that the value of a property should compared to the value of another named property on the same type
/// using the specified <see cref="Operator"/>.
/// </summary>
/// <seealso cref="ContingentValidationAttribute" />
public class IsAttribute : ContingentValidationAttribute
{
	private readonly OperatorMetadata _metadata;

	/// <summary>
	/// Gets the operator.
	/// </summary>
	public EqualityOperator Operator { get; private set; }

	/// <summary>
	/// Gets or sets a value indicating whether to pass on null.
	/// </summary>
	public bool PassOnNull { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="IsAttribute"/> class.
	/// </summary>
	/// <param name="operator">The operator.</param>
	/// <param name="dependentProperty">The dependent property.</param>
	public IsAttribute(EqualityOperator @operator, string dependentProperty)
		: base(dependentProperty)
	{
		Operator = @operator;
		PassOnNull = false;
		_metadata = OperatorMetadata.Get(Operator);
	}

	/// <inheritdoc />
	public override string ClientTypeName => "Is";

	/// <inheritdoc />
	protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters() => base.GetClientValidationParameters()
			.Union(new[]
				   {
					   new KeyValuePair<string, object>("Operator", Operator.ToString()),
					   new KeyValuePair<string, object>("PassOnNull", PassOnNull)
				   });

	/// <inheritdoc />
	public override bool IsValid(object value, object? actualDependentPropertyValue, object model)
	{
		if (PassOnNull && (value is null || actualDependentPropertyValue is null))
			return true;

		return _metadata.IsValid(value, actualDependentPropertyValue, ReturnTrueOnEitherNull);
	}

	/// <inheritdoc />
	public override string DefaultErrorMessageFormat => "{0} must be " + _metadata.ErrorMessage + " {1}.";
}