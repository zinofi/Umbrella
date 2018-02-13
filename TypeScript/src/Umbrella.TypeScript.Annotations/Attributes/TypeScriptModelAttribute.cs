using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.TypeScript
{
    /// <summary>
    /// Used to mark models (classes or interfaces) to be output by the TypeScript generator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = false)]
    public class TypeScriptModelAttribute : Attribute
    {
        /// <summary>
        /// The types of model that the generator should output.
        /// </summary>
        public TypeScriptOutputModelType OutputModelTypes { get; set; }
        
        /// <summary>
        /// Specifies whether the generator should output validation rules for the specified output model types.
        /// This is currently only supported for <see cref="TypeScriptOutputModelType.AureliaClass"/> models.
        /// </summary>
        public bool GenerateValidationRules { get; set; }

        /// <summary>
        /// The constructor for the attribute allowing the values for the
        /// <see cref="OutputModelTypes"/> and <see cref="GenerateValidationRules"/> properties
        /// to be provided as arguments.
        /// </summary>
        /// <param name="modelTypes">
        /// The types of model that the generator should output.
        /// </param>
        /// <param name="generateValidationRules">
        /// Specifies whether the generator should output validation rules for the specified output model types.
        /// This is currently only supported for <see cref="TypeScriptOutputModelType.AureliaClass"/> models.
        /// </param>
        public TypeScriptModelAttribute(TypeScriptOutputModelType modelTypes, bool generateValidationRules = true)
        {
            OutputModelTypes = modelTypes;
            GenerateValidationRules = generateValidationRules;
        }
    }
}