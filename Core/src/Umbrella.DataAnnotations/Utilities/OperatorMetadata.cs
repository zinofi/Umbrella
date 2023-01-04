using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Umbrella.DataAnnotations.Utilities;

/// <summary>
/// Defines the metadata for contingent validation operations.
/// </summary>
public class OperatorMetadata
{
	private static readonly Dictionary<EqualityOperator, OperatorMetadata> _operatorMetadata;

	/// <summary>
	/// Gets the error message.
	/// </summary>
	public string ErrorMessage { get; private set; } = null!;

	/// <summary>
	/// Returns true if the validation check passes.
	/// </summary>
	public Func<object, object, bool, bool> IsValid { get; private set; } = null!;

	/// <summary>
	/// Initializes the <see cref="OperatorMetadata"/> class.
	/// </summary>
	static OperatorMetadata()
	{
		_operatorMetadata = new Dictionary<EqualityOperator, OperatorMetadata>()
		{
			{
				EqualityOperator.EqualTo, new OperatorMetadata()
				{
					ErrorMessage = "equal to",
					IsValid = (value, dependentValue, returnTrueOnEitherNull) =>
					{
						if((value is null || dependentValue is null) && returnTrueOnEitherNull)
							return true;

						if (value is null && dependentValue is null)
							return true;
						else if (value is null && dependentValue is not null)
							return false;

						return value?.Equals(dependentValue) is true;
					}
				}
			},
			{
				EqualityOperator.NotEqualTo, new OperatorMetadata()
				{
					ErrorMessage = "not equal to",
					IsValid = (value, dependentValue, returnTrueOnEitherNull) =>
					{
						if((value is null || dependentValue is null) && returnTrueOnEitherNull)
							return true;

						if (value is null && dependentValue is not null)
							return true;
						else if (value is null && dependentValue is null)
							return false;

						return value?.Equals(dependentValue) is false;
					}
				}
			},
			{
				EqualityOperator.GreaterThan, new OperatorMetadata()
				{
					ErrorMessage = "greater than",
					IsValid = (value, dependentValue, returnTrueOnEitherNull) =>
					{
						if((value is null || dependentValue is null) && returnTrueOnEitherNull)
							return true;

						if (value is null || dependentValue is null)
							return false;

						return Comparer<object>.Default.Compare(value, dependentValue) >= 1;
					}
				}
			},
			{
				EqualityOperator.LessThan, new OperatorMetadata()
				{
					ErrorMessage = "less than",
					IsValid = (value, dependentValue, returnTrueOnEitherNull) =>
					{
						if((value is null || dependentValue is null) && returnTrueOnEitherNull)
							return true;

						if (value is null || dependentValue is null)
							return false;

						return Comparer<object>.Default.Compare(value, dependentValue) <= -1;
					}
				}
			},
			{
				EqualityOperator.GreaterThanOrEqualTo, new OperatorMetadata()
				{
					ErrorMessage = "greater than or equal to",
					IsValid = (value, dependentValue, returnTrueOnEitherNull) =>
					{
						if((value is null || dependentValue is null) && returnTrueOnEitherNull)
							return true;

						if (value is null && dependentValue is null)
							return true;

						if (value is null || dependentValue is null)
							return false;

						return Get(EqualityOperator.EqualTo).IsValid(value, dependentValue, returnTrueOnEitherNull) || Comparer<object>.Default.Compare(value, dependentValue) >= 1;
					}
				}
			},
			{
				EqualityOperator.LessThanOrEqualTo, new OperatorMetadata()
				{
					ErrorMessage = "less than or equal to",
					IsValid = (value, dependentValue, returnTrueOnEitherNull) =>
					{
						if((value is null || dependentValue is null) && returnTrueOnEitherNull)
							return true;

						if (value is null && dependentValue is null)
							return true;

						if (value is null || dependentValue is null)
							return false;

						return Get(EqualityOperator.EqualTo).IsValid(value, dependentValue, returnTrueOnEitherNull) || Comparer<object>.Default.Compare(value, dependentValue) <= -1;
					}
				}
			},
			{
				EqualityOperator.RegExMatch, new OperatorMetadata()
				{
					ErrorMessage = "a match to",
					IsValid = (value, dependentValue, returnTrueOnEitherNull) =>
					{
						if((value is null || dependentValue is null) && returnTrueOnEitherNull)
							return true;

						return Regex.Match((value ?? "").ToString(), dependentValue?.ToString() ?? "").Success;
					}
				}
			},
			{
				EqualityOperator.NotRegExMatch, new OperatorMetadata()
				{
					ErrorMessage = "not a match to",
					IsValid = (value, dependentValue, returnTrueOnEitherNull) =>
					{
						if((value is null || dependentValue is null) && returnTrueOnEitherNull)
							return true;

						return !Regex.Match((value ?? "").ToString(), dependentValue?.ToString() ?? "").Success;
					}
				}
			}
		};
	}

	/// <summary>
	/// Gets the metadata for specified <paramref name="operator"/>.
	/// </summary>
	/// <param name="operator">The operator.</param>
	/// <returns>The <see cref="OperatorMetadata"/>.</returns>
	public static OperatorMetadata Get(EqualityOperator @operator) => _operatorMetadata[@operator];
}