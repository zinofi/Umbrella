using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.TypeScript
{
    public class TypeScriptMemberInfo
    {
        public bool IsNullable { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public Type CLRType { get; set; }
        public string InitialOutputValue { get; set; }
    }
}
