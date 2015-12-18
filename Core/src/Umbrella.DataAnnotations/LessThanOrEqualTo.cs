using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.DataAnnotations
{
    public class LessThanOrEqualToAttribute : IsAttribute
    {
        public LessThanOrEqualToAttribute(string dependentProperty) : base(Operator.LessThanOrEqualTo, dependentProperty) { }
    }
}
