using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// A stricter version of the <see cref="MinLengthAttribute" /> which also fails validation
	/// when the collection is null. That isn't the case with the <see cref="MinLengthAttribute" />.
	/// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredNonEmptyCollectionAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is ICollection collection && collection.Count > 0)
                return ValidationResult.Success;

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}