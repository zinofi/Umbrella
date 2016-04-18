using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.TypeScript.Attributes;
using Umbrella.WebUtilities.TypeScript.Enumerations;
using Umbrella.WebUtilities.TypeScript.Generators.Interfaces;

namespace Umbrella.WebUtilities.TypeScript.Generators
{
    public class StandardClassGenerator : BaseClassGenerator
    {
        public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.Class;

        protected override TypeScriptOutputModelType InterfaceModelType => TypeScriptOutputModelType.Interface;
    }
}
