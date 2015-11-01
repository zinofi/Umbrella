using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.WebUtilities.TypeScript.Enumerations;

namespace Umbrella.WebUtilities.TypeScript.Generators.Interfaces
{
    public interface IGenerator
    {
        TypeScriptOutputModelType OutputModelType { get; }
        string Generate(Type modelType);
    }
}
