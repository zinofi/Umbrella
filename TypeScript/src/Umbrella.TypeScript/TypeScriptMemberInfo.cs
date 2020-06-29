using System;

namespace Umbrella.TypeScript
{
	public class TypeScriptMemberInfo
	{
		public bool IsNullable { get; set; }
		public string Name { get; set; }
		public string TypeName { get; set; }
		public Type CLRType { get; set; }
		public string InitialOutputValue { get; set; }
		public bool IsUserDefinedType { get; set; }
	}
}