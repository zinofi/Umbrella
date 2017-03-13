using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.TypeScript.Aurelia
{
    public static class TypeScriptGeneratorExtensions
    {
        public static TypeScriptGenerator IncludeAureliaGenerators(this TypeScriptGenerator generator)
        {
            generator.Generators.Add(new AureliaInterfaceGenerator());
            generator.Generators.Add(new AureliaClassGenerator());

            return generator;
        }
    }
}