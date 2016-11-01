using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations
{
    public class UmbrellaPhoneAttribute : RegularExpressionAttribute
    {
        public UmbrellaPhoneAttribute()
            : base(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$")
        {
        }
    }
}