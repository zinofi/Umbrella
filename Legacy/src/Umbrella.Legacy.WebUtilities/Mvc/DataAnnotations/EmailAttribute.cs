using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.DataAnnotations
{
    public class EmailAttribute : RegularExpressionAttribute
    {
        static EmailAttribute()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(EmailAttribute), typeof(RegularExpressionAttributeAdapter));
        }

        public EmailAttribute()
            : base("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,10}|[0-9]{1,3})(\\]?)$")
        {
            
        }
    }
}