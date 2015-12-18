using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.WebUtilities.TypeScript.Attributes;

namespace Umbrella.WebUtilities.TypeScript
{
    public class TypeScriptModelGeneratorItem
    {
        public Type ModelType { get; set; }
        public TypeScriptModelAttribute ModelAttribute { get; set; }
    }
}
