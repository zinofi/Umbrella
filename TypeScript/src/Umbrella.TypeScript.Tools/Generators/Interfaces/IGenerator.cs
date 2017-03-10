using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.TypeScript.Tools.Generators.Interfaces
{
    public interface IGenerator
    {
        TypeScriptOutputModelType OutputModelType { get; }
        string Generate(Type modelType, bool generateValidationRules, bool strictNullChecks, TypeScriptPropertyMode propertyMode);
    }
}
