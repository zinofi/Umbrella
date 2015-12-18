using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.DataAnnotations
{
    public class RequiredIfRegExMatchAttribute : RequiredIfAttribute
    {
        public RequiredIfRegExMatchAttribute(string dependentProperty, string pattern) : base(dependentProperty, Operator.RegExMatch, pattern) { }
    }
}
