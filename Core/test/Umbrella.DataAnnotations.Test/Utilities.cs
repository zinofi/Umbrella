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
            return (T)GetType().GetProperty(property).GetCustomAttributes(typeof(T), false).First();
        }

        public bool IsValid(string property) 
        {
            var attribute = GetAttribute(property);
            return attribute.IsValid(GetType().GetProperty(property).GetValue(this, null), this);
        }
    }
}