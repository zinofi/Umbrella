using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Umbrella.DataAnnotations.BaseClasses
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ContingentValidationAttribute : ModelAwareValidationAttribute
    {
        public string DependentProperty { get; private set; }

        public ContingentValidationAttribute(string dependentProperty)
        {
            DependentProperty = dependentProperty;
        }
        
        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrEmpty(ErrorMessageResourceName) && string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = DefaultErrorMessage;
            
            return string.Format(ErrorMessageString, name, DependentProperty);
        }

		public override string DefaultErrorMessage => "{0} is invalide due to {1}.";

		private object GetDependentPropertyValue(object container)
        {
			TypeInfo currentType = container.GetType().GetTypeInfo();
			object value = container;

            foreach (string propertyName in DependentProperty.Split('.'))
            {
				PropertyInfo property = currentType.GetProperty(propertyName);
                value = property.GetValue(value, null);
                currentType = property.PropertyType.GetTypeInfo();
            }

            return value;
        }

		protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters() => base.GetClientValidationParameters()
				.Union(new[] { new KeyValuePair<string, object>("DependentProperty", DependentProperty) });

		public override bool IsValid(object value, object container) => IsValid(value, GetDependentPropertyValue(container), container);

		public abstract bool IsValid(object value, object dependentValue, object container);
    }
}