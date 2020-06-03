using System.ComponentModel.DataAnnotations;
using Umbrella.DataAnnotations.Utilities;

namespace Umbrella.DataAnnotations
{
	public class RequiredNonEmptyCollectionIfAttribute : RequiredIfAttribute
	{
		public RequiredNonEmptyCollectionIfAttribute(string dependentProperty, Operator @operator, bool dependentValue)
			: base(dependentProperty, @operator, dependentValue)
		{
		}

		public override bool IsValid(object value, object dependentValue, object container, ValidationContext validationContext)
		{
			if (Metadata.IsValid(dependentValue, DependentValue))
				return ValidationHelper.IsNonEmptyCollection(value, validationContext);

			return true;
		}
	}
}