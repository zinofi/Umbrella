using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.DataAnnotations
{
    public class RequiredIfNotAttribute : RequiredIfAttribute
    {
        public RequiredIfNotAttribute(string dependentProperty, object dependentValue) : base(dependentProperty, Operator.NotEqualTo, dependentValue) { }
    }
}
