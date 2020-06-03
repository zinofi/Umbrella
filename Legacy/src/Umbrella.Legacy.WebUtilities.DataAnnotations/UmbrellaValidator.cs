using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.Legacy.WebUtilities.DataAnnotations
{
    public class UmbrellaValidator : DataAnnotationsModelValidator<ModelAwareValidationAttribute>
    {
        public UmbrellaValidator(ModelMetadata metadata, ControllerContext context, ModelAwareValidationAttribute attribute)
            : base(metadata, context, attribute) { }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            if (!Attribute.IsValid(Metadata.Model, container, new ValidationContext(container)))
                yield return new ModelValidationResult { Message = ErrorMessage };                    
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var result = new ModelClientValidationRule()
            {
                ValidationType = Attribute.ClientTypeName.ToLower(),
                ErrorMessage = ErrorMessage       
            };
            
            foreach (var validationParam in Attribute.ClientValidationParameters)
                result.ValidationParameters.Add(validationParam);
            
            yield return result;
        }
    }
}
