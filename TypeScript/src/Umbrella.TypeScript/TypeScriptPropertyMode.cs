using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.TypeScript
{
    public enum TypeScriptPropertyMode
    {
        /// <summary>
        /// This results in a property definition output of: propertyName: typeName;
        /// </summary>
        None = 0,
        /// <summary>
        /// This results in a property definition output of: propertyName: typeName | null = null;
        /// | null will only be emitted when strict null checks are enabled.
        /// </summary>
        Null = 1,
        /// <summary>
        /// This results in a property definition output of: propertyName: typeName = defaultValue;
        /// This may include | null if strict null checks are enabled and the defaultValue is null.
        /// </summary>
        Model = 2
    }
}