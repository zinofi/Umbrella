using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Validators
{
    public class MinDateAttribute : ValidationAttribute, IClientValidatable
    {
        private DateTime m_MinDate;

        /// <summary>
        /// Minimum date defaults to the current date
        /// </summary>
        public MinDateAttribute()
        {
            m_MinDate = DateTime.Now.Date;
        }

        /// <summary>
        /// Minimum date based on the offset days from the current date
        /// </summary>
        /// <param name="offset"></param>
        public MinDateAttribute(int offset)
        {
            m_MinDate = DateTime.Now.AddDays(offset);
        }

        /// <summary>
        /// Minimum date based on the exact date specified
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public MinDateAttribute(int year, int month, int day)
        {
            m_MinDate = new DateTime(year, month, day);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string strDateTime = value as string;

            if(!string.IsNullOrEmpty(strDateTime))
            {
                //We have a value - try and parse it to a datetime
                DateTime result;
                if (DateTime.TryParse(strDateTime, out result) && result.Date < m_MinDate)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            //Assume success
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "datemin";
            rule.ValidationParameters.Add("min", m_MinDate.ToShortDateString());
            yield return rule;
        }
    }
}