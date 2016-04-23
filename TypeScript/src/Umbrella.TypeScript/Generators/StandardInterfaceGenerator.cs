using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.TypeScript.Attributes;
using Umbrella.TypeScript.Enumerations;
using Umbrella.TypeScript.Generators.Interfaces;

namespace Umbrella.TypeScript.Generators
{
    public class StandardInterfaceGenerator : BaseInterfaceGenerator
    {
        public override TypeScriptOutputModelType OutputModelType
        {
            get { return TypeScriptOutputModelType.Interface; }
        }
    }
}