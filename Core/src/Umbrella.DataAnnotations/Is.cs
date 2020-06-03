using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Umbrella.DataAnnotations.BaseClasses;
using Umbrella.DataAnnotations.Utilities;

namespace Umbrella.DataAnnotations
{
	public class IsAttribute : ContingentValidationAttribute
	{
		public Operator Operator { get; private set; }
		public bool PassOnNull { get; set; }
		private readonly OperatorMetadata _metadata;

		public IsAttribute(Operator @operator, string dependentProperty)
			: base(dependentProperty)
		{
			Operator = @operator;
			PassOnNull = false;
			_metadata = OperatorMetadata.Get(Operator);
		}

		public override string ClientTypeName => "Is";

		protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters() => base.GetClientValidationParameters()
				.Union(new[]
					   {
						   new KeyValuePair<string, object>("Operator", Operator.ToString()),
						   new KeyValuePair<string, object>("PassOnNull", PassOnNull)
					   });

		public override bool IsValid(object value, object dependentValue, object container, ValidationContext validationContext)
		{
			if (PassOnNull && (value == null || dependentValue == null))
				return true;

			return _metadata.IsValid(value, dependentValue);
		}

		public override string DefaultErrorMessage => "{0} must be " + _metadata.ErrorMessage + " {1}.";
	}
}
