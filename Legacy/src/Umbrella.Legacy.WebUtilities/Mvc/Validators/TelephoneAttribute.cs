using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Validators
{
    public class TelephoneAttribute : RegularExpressionAttribute
    {
        static TelephoneAttribute()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(TelephoneAttribute), typeof(RegularExpressionAttributeAdapter));
        }

        public TelephoneAttribute()
            : base(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$")
        {

        }
    }
}
