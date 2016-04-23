using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.TypeScript.Attributes
{
    /// <summary>
    /// Used to mark enum types to be output by the TypeScript generator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class TypeScriptEnumAttribute : Attribute
    {
    }
}