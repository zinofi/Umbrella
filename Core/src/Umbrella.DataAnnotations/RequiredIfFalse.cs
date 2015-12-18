using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.DataAnnotations
{
    public class RequiredIfFalseAttribute : RequiredIfAttribute
    {
        public RequiredIfFalseAttribute(string dependentProperty) : base(dependentProperty, Operator.EqualTo, false) { }
    }
}
