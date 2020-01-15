using System.Collections.Generic;

namespace Umbrella.TypeScript.Tools
{
	public class ToolOptions
	{
		public bool VerboseEnabled { get; set; }
		public bool DebugEnabled { get; set; }
		public string AssemblyFolderPath { get; set; }
		public List<string> AssemblyNameList { get; set; }
		public List<string> RuntimeAssemblyNameList { get; set; }
		public List<string> GeneratorList { get; set; }
		public string OutputType { get; set; }
		public bool StrictNullChecks { get; set; }
		public TypeScriptPropertyMode PropertyMode { get; set; }
		public string OutputPath { get; set; }
		public bool KnockoutUseDecorators { get; set; }
		public bool ValidationEnabled { get; set; }
	}
}