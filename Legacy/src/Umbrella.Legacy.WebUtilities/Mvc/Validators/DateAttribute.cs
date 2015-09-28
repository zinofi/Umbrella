using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Validators
{
    public class DateAttribute : RegularExpressionAttribute
    {
        static DateAttribute()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DateAttribute), typeof(RegularExpressionAttributeAdapter));
        }

        public DateAttribute()
            : base(@"^\d{1,2}[\/-]\d{1,2}[\/-]\d{4}$")
        {
            
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                string date = null;

                if (value is string)
                {
                    date = value as string;
                }
                else if (value is DateTime)
                {
                    date = ((DateTime)value).ToShortDateString();
                }
                else if (value is DateTime?)
                {
                    DateTime? dt = ((DateTime?)value);
                    if (dt.HasValue)
                        date = dt.Value.ToShortDateString();
                }

                if(!string.IsNullOrEmpty(date))
                    return Regex.IsMatch(date, Pattern);
            }

            return true;
        }
    }
}