using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Umbrella.WebUtilities.TypeScript.Enumerations;

namespace Umbrella.WebUtilities.TypeScript.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class TypeScriptModelAttribute : Attribute
    {
        public TypeScriptOutputModelType OutputModelTypes { get; set; }

        public TypeScriptModelAttribute(TypeScriptOutputModelType modelTypes)
        {
            OutputModelTypes = modelTypes;
        }
    }
}
