using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.TypeScript.Generators.Interfaces;

namespace Umbrella.TypeScript.Generators
{
    public class StandardClassGenerator : BaseClassGenerator
    {
        public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.Class;
        protected override bool SupportsValidationRules => false;
        protected override TypeScriptOutputModelType InterfaceModelType => TypeScriptOutputModelType.Interface;
    }
}
