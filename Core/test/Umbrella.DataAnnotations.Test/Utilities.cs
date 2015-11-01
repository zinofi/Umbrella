using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.DataAnnotations.Test
{
    abstract class ModelBase<T> where T: ContingentValidationAttribute
    {
        public T GetAttribute(string property) 
        {
            return (T)this.GetType().GetProperty(property).GetCustomAttributes(typeof(T), false)[0];
        }

        public bool IsValid(string property) 
        {
            var attribute = this.GetAttribute(property);
            return attribute.IsValid(this.GetType().GetProperty(property).GetValue(this, null), this);
        }
    }
}
