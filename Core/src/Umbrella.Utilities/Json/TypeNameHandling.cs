using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Json
{
    /// <summary>
    /// Specifies Type Name handling options when serializing / deserializing objects. This enum has been lifted from
    /// the Newtonsoft.Json code and is here as part of the abstraction efforts to avoid a hard dependency on Newtonsoft.Json.
    /// </summary>
    [Flags]
    public enum TypeNameHandling
    {
        /// <summary>
        /// Do not include the .NET type name when serializing types.
        /// </summary>
        None = 0,

        /// <summary>
        /// Include the .NET type name when serializing into a JSON object structure.
        /// </summary>
        Objects = 1,

        /// <summary>
        /// Include the .NET type name when serializing into a JSON array structure.
        /// </summary>
        Arrays = 2,

        /// <summary>
        /// Always include the .NET type name when serializing.
        /// </summary>
        All = 3,
    }
}