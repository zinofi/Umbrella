using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.DataAnnotations
{
    public class LessThanAttribute : IsAttribute
    {
        public LessThanAttribute(string dependentProperty) : base(Operator.LessThan, dependentProperty) { }
    }
}
