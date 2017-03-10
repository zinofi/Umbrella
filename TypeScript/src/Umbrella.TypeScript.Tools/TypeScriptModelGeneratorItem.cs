using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.TypeScript.Tools
{
    public class TypeScriptModelGeneratorItem
    {
        public Type ModelType { get; set; }
        public TypeScriptModelAttribute ModelAttribute { get; set; }
    }
}
