using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations.BaseClasses
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ModelAwareValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            throw new NotImplementedException();
        }

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrEmpty(ErrorMessageResourceName) && string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = DefaultErrorMessage;
            
            return base.FormatErrorMessage(name);
        }

        public virtual string DefaultErrorMessage
        {
            get { return "{0} is invalid."; }
        }

        public abstract bool IsValid(object value, object container, ValidationContext validationContext);

        public virtual string ClientTypeName
        {
            get { return GetType().Name.Replace("Attribute", ""); }
        }

        protected virtual IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters()
        {
            return new KeyValuePair<string, object>[0];
        }
        
        public Dictionary<string, object> ClientValidationParameters
        {
            get { return GetClientValidationParameters().ToDictionary(kv => kv.Key.ToLower(), kv => kv.Value); }
        }

        protected sealed override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
			object container = validationContext.ObjectInstance;

            if (!IsValid(value, container, validationContext))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.MemberName == null ? null : new [] { validationContext.MemberName });

            return ValidationResult.Success;
        }
    }
}
