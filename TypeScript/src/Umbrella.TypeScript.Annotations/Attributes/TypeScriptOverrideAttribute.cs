using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.TypeScript.Annotations.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TypeScriptOverrideAttribute : Attribute
    {
        public Type TypeOverride { get; set; }

        public TypeScriptOverrideAttribute(Type typeOverride)
        {
            TypeOverride = typeOverride;
        }
    }
}