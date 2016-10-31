using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.DataAnnotations
{
    public class UmbrellaPostcodeAttribute : RegularExpressionAttribute
    {
        public UmbrellaPostcodeAttribute()
            : base("^((([A-Pa-pR-UWYZr-uwyz](\\d([A-HJKSTUWa-hjkstuw]|\\d)?|" +
                                          "[A-Ha-hK-Yk-y]\\d([AaBbEeHhMmNnPpRrVvWwXxYy]|\\d)?))\\s*" +
                                          "(\\d[ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2})?)|[Gg][Ii][Rr]\\s*0[Aa][Aa])$")
        {
        }
    }
}
